using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Himitsu.Pages
{
    public class mainController : Controller
    {
        private MySqlDataReader reader;
        private MySqlCommand cmd;
        private readonly MySqlConnection _sql;

        public mainController(MySqlConnection sql)
        {
            _sql = sql;
        }
        public struct Data
        {
            public int ID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Country { get; set; }
            public ulong pRaw { get; set; }
            public UserPrivileges Privileges { get; set; }
        }
        public IActionResult Leaderboard()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Вы не ввели логин/пароль!";
                return View("error403");
            }

            var data = new Data();
            var user_safe = username.ToString().ToLowerInvariant().Replace(" ", "_");
            cmd = new MySqlCommand("SELECT u.id, u.password_md5, u.username, s.country, u.privileges FROM users u LEFT JOIN users_stats s ON s.id = u.id WHERE u.username_safe = @user_safe LIMIT 1", _sql);
            cmd.Parameters.AddWithValue("@user_safe", user_safe);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                data.ID = Convert.ToInt32(reader["id"]);
                data.Username = reader["username"].ToString();
                data.Password = reader["password_md5"].ToString();
                data.Country = reader["country"].ToString();
                data.pRaw = Convert.ToUInt64(reader["privileges"]);
                data.Privileges = (UserPrivileges)data.pRaw;
                reader.Close();
            }

            if (data.Username == null)
            {
                ViewBag.Error = "Пользователя с таким ником не существует!";
                return View("error403");
            }

            if (!BCrypt.Net.BCrypt.Verify(Utility.CreateMD5(password).ToLowerInvariant(), data.Password))
            {
                ViewBag.Error = "Неверный пароль.";
                return View("error403");
            }

            var s = HttpContext.Session;
            if (UserPrivileges.Pending == data.Privileges)
            {
                Utility.setCookie(_sql, HttpContext, data.ID);
                s.CommitAsync();
                return RedirectToAction("Verify", "register");
            }

            if (UserPrivileges.Banned == data.Privileges)
            {
                ViewBag.Error = "Ваш аккаунт был заблокирован/забанен. Обратитесь к Администрации.";
                return View("error403");
            }
            Utility.setCookie(_sql, HttpContext, data.ID);
            s.SetInt32("userid", data.ID);
            s.SetString("pw", Utility.CreateMD5(data.Password).ToLowerInvariant());
            s.SetString("logout", Utility.GenerateString());

            afterLogin(data.ID, s, data.Country);
            return RedirectToAction("Main", "main");
        }
        public void afterLogin(int user_id, ISession sess, string country)
        {
            var s = GenerateToken(user_id);
            sess.SetString("token", s);
            if (country == "XX")
                Utility.setCountry(_sql, HttpContext, user_id);
            Utility.LogIP(_sql, HttpContext, user_id);
        }
        public string GenerateToken(int user_id)
        {
            var rs = Utility.GenerateString(32);
            var cmd = new MySqlCommand("INSERT INTO tokens(user, privileges, description, token, private) VALUES(@id, '0', @ip, @md5, '1')", _sql);
            cmd.Parameters.AddWithValue("@id", user_id);
            cmd.Parameters.AddWithValue("@ip", HttpContext.Connection.RemoteIpAddress);
            cmd.Parameters.AddWithValue("@md5", Utility.CreateMD5(rs).ToLowerInvariant());
            cmd.ExecuteNonQuery();
            return rs;
        }
        public IActionResult Main()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View("error404");
        }

        public IActionResult Switcher()
        {
            string file_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resources", "HimitsuSwitcher.exe");
            return PhysicalFile(file_path, System.Net.Mime.MediaTypeNames.Application.Octet, "HimitsuSwitcher.exe");
        }
    }
}
