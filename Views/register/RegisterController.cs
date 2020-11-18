using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt;
using SqlKata.Execution;

namespace Himitsu.Views.register
{
    public class registerController : Controller
    {
        private readonly QueryFactory _db;

        public registerController(QueryFactory db)
        {
            _db = db;
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

            string check = _db.Query("identity_tokens").SelectRaw("SELECT token FROM identity_tokens WHERE token = @token AND userid = @user_id", new { token = Request.Cookies["y"], user_id = u }).FirstOrDefault();
            if (check != null)
            {
                ViewBag.Error = "Зачем ты это делаешь?";
                return View("error403");
            }

            long check_priv = _db.Query("users").Select("privileges").Where("id", u).First().privileges;
            if (check_priv != Convert.ToInt64(UserPrivileges.Pending))
            {
                ViewBag.Error = "Что ты тут забыл?";
                return View("error403");
            }

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
                    "ztrot", "koreapenguin", "fort", "asphyxia", "niko", "shigetora", "whitecat", "fokabot", "himitsu", "nebula", "howl", "nyo", "angelwar", "mm00"};
            
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

            dynamic check = _db.Query("users").Select("username_safe").Where("username_safe", username.ToString().ToLowerInvariant().Replace(" ", "_")).FirstOrDefault();
            if (check != null)
            {
                ViewBag.Error = "Такой пользователь уже существует!";
                return View("error403");
            }
            
            check = _db.Query("users").Select("email").Where("email", mail).FirstOrDefault();
            if (check != null)
            {
                ViewBag.Error = "Эта почта уже занята!";
                return View("error403");
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(EncryptProvider.Md5(password).ToLowerInvariant(), 10);

            _db.Query("users").Insert(new { username, username_safe = username.ToString().ToLowerInvariant(), password_md5 = hash, email = mail, register_datetime = DateTime.UnixEpoch, privileges = UserPrivileges.Pending });

            int user_id = _db.Query("users").Select("id").Where("email", mail).Where("password_md5", hash).First().id;

            _db.Query("users_stats").Insert(new { id = user_id, username, user_color = "black", user_style = "", ranked_score_std = 0, playcount_std = 0, total_score_std = 0, ranked_score_taiko = 0, playcount_taiko = 0, total_score_taiko = 0, ranked_score_ctb = 0, playcount_ctb = 0, total_score_ctb = 0, ranked_score_mania = 0, playcount_mania = 0, total_score_mania = 0 });
            _db.Query("users_stats_relax").Insert(new { id = user_id });
            _db.Query("users_preferences").Insert(new { id = user_id });

            Utility.setCookie(_db, HttpContext, user_id);
            HttpContext.Session.CommitAsync();
            return RedirectToAction("Verify", "register", new { u = user_id });
        }
        private bool registerEnabled()
        {
            int check = _db.Query("system_settings").Select("value_int").Where("name", "registrations_enabled").First().value_int;
            return Utility.IntToBool(check);
        }
    }
}
