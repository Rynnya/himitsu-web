using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Himitsu.Views.register
{
    public class registerController : Controller
    {
        private MySqlDataReader reader;
        private MySqlCommand cmd;
        private readonly MySqlConnection _sql;

        public registerController(MySqlConnection sql)
        {
            _sql = sql;
        }
        public IActionResult Verify(int? u)
        {
            if (u == 0 || string.IsNullOrEmpty(u.ToString()))
            {
                ViewBag.Error = "Не-а.";
                return View("error403");
            }
            ViewBag.User = u;
            var s = HttpContext.Session;
            if (s.Keys.Contains("userid"))
            {
                ViewBag.Error = "Ты здесь уже был.";
                return View("error403");
            }

            cmd = new MySqlCommand("SELECT 1 FROM identity_tokens WHERE token = @token AND userid = @user_id", _sql);
            cmd.Parameters.AddWithValue("@token", Request.Cookies["y"]);
            cmd.Parameters.AddWithValue("@user_id", u);
            reader = cmd.ExecuteReader();
            if (!(reader.Read() && reader["1"] != null))
            {
                reader.Close();
                ViewBag.Error = "Зачем ты это делаешь?";
                return View("error403");
            }
            reader.Close();

            cmd = new MySqlCommand("SELECT privileges FROM users WHERE id = @user_id", _sql);
            cmd.Parameters.AddWithValue("@user_id", u);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (reader["privileges"].ToString() != UserPrivileges.Pending.ToString())
                {
                    reader.Close();
                    ViewBag.Error = "Что ты тут забыл?";
                    return View("error403");
                }
            }
            else { reader.Close(); return View("error500"); }
            reader.Close();
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string mail, string password)
        {
            string[] forbiddenUsernames = new string[] { "peppy", "rrtyui", "cookiezi", "azer", "loctav", "banchobot", "happystick", "doomsday", "sharingan33", "andrea", "cptnxn",
                    "reimu-desu", "hvick225", "_index", "my aim sucks", "kynan", "rafis", "sayonara-bye", "thelewa", "wubwoofwolf", "millhioref", "tom94", "tillerino", "clsw",
                    "spectator", "exgon", "axarious", "angelsim", "recia", "nara", "emperorpenguin83", "bikko", "xilver", "vettel", "kuu01", "_yu68", "tasuke912", "dusk",
                    "ttobas", "velperk", "jakads", "jhlee0133", "abcdullah", "yuko-", "entozer", "hdhr", "ekoro", "snowwhite", "osuplayer111", "musty", "nero", "elysion",
                    "ztrot", "koreapenguin", "fort", "asphyxia", "niko", "shigetora", "whitecat", "fokabot", "himitsu", "nebula"};
            
            if (!registerEnabled())
            {
                ViewBag.Error = "Простите, но сейчас регистрация недоступна.";
                return View("error403");
            }

            username = username.ToString().Trim();
            Regex usernameRegex = new Regex(@"^[A-Za-z0-9 _\[\]-]{2,15}$");
            if (!usernameRegex.IsMatch(username))
            {
                ViewBag.Error = "Ваш ник может состоять только из букв алфавит, цифр, и символов -[]_";
                return View("error403");
            }

            foreach (var i in forbiddenUsernames)
                if (username.ToString().ToLowerInvariant() == i)
                {
                    ViewBag.Error = "Этот ник запрещен.";
                    return View("error403");
                }

            if (username.Contains("_") && username.Contains(" "))
            {
                ViewBag.Error = "Ник не должен содержать одновременно пробелы и нижние подчеркивания.";
                return View("error403");
            }

            cmd = new MySqlCommand("SELECT 1 FROM users WHERE username_safe = @username", _sql);
            cmd.Parameters.AddWithValue("@username", username.ToString().ToLowerInvariant().Replace(" ", "_"));
            reader = cmd.ExecuteReader();
            if (reader.Read())
                if (reader["1"] != null)
                {
                    ViewBag.Error = "Такой пользователь уже существует!";
                    reader.Close();
                    return View("error403");
                }
            reader.Close();

            cmd = new MySqlCommand("SELECT 1 FROM users WHERE email = @email", _sql);
            cmd.Parameters.AddWithValue("@email", mail);
            reader = cmd.ExecuteReader();
            if (reader.Read())
                if (reader["1"] != null)
                {
                    ViewBag.Error = "Эта почта уже занята!";
                    reader.Close();
                    return View("error403");
                }
            reader.Close();

            var hash = BCrypt.Net.BCrypt.HashPassword(Utility.CreateMD5(password).ToLowerInvariant(), 10);

            cmd = new MySqlCommand("INSERT INTO users(username, username_safe, password_md5, salt, email, register_datetime, privileges, password_version) VALUES( @username, @username_safe, @hash, '', @email, @date, @privileges, 2); ", _sql);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@username_safe", username.ToString().ToLowerInvariant());
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.Parameters.AddWithValue("@email", mail);
            cmd.Parameters.AddWithValue("@date", DateTime.UnixEpoch);
            cmd.Parameters.AddWithValue("@privileges", UserPrivileges.Pending);
            cmd.ExecuteNonQuery();

            int user_id = 0;
            cmd = new MySqlCommand($"SELECT id FROM users WHERE email = @mail AND password_md5 = @hash");
            cmd.Parameters.AddWithValue("@mail", mail);
            cmd.Parameters.AddWithValue("@hash", hash);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user_id = Convert.ToInt32(reader["id"]);
                reader.Close();

                cmd = new MySqlCommand("INSERT INTO `users_stats`(id, username, user_color, user_style, ranked_score_std, playcount_std, total_score_std, ranked_score_taiko, playcount_taiko, total_score_taiko, ranked_score_ctb, playcount_ctb, total_score_ctb, ranked_score_mania, playcount_mania, total_score_mania) VALUES (@id, @username, 'black', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)", _sql);
                cmd.Parameters.AddWithValue("@id", user_id);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("INSERT INTO `users_stats_relax` (id) VALUES (@id)", _sql);
                cmd.Parameters.AddWithValue("@id", user_id);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("INSERT INTO `users_preferences` (id) VALUES (@id)", _sql);
                cmd.Parameters.AddWithValue("@id", user_id);
                cmd.ExecuteNonQuery();

                Utility.setCookie(_sql, HttpContext, user_id);
                Utility.LogIP(_sql, HttpContext, user_id);
                HttpContext.Session.CommitAsync();
            }
            return RedirectToAction("Verify", "register", new { u = user_id });
        }
        private bool registerEnabled()
        {
            reader = new MySqlCommand("SELECT value_int FROM system_settings WHERE name = 'registrations_enabled'", _sql).ExecuteReader();
            if (reader.Read())
            {
                var done = reader["value_int"].ToString();
                reader.Close();
                return Utility.StringToBool(done);
            }
            reader.Close();
            return false;
        }
    }
}
