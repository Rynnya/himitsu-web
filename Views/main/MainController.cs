using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt;
using SqlKata.Execution;

namespace Himitsu.Pages
{
    [Route("")]
    public class mainController : Controller
    {
        private readonly QueryFactory _db;

        public mainController(QueryFactory db)
        {
            _db = db;
        }
        public struct Data
        {
            public int ID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Country { get; set; }
            public long pRaw { get; set; }
            public UserPrivileges Privileges { get; set; }
        }

        [HttpGet("leaderboard")]
        public IActionResult Leaderboard()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Вы не ввели логин/пароль!";
                return View("error403");
            }

            Data data = new Data();
            string user_safe = username.ToString().ToLowerInvariant().Replace(" ", "_");
            dynamic user_data = _db.Select("SELECT u.id, u.password_md5, u.username, s.country, u.privileges FROM users u LEFT JOIN users_stats s ON s.id = u.id WHERE u.username_safe = @user_safe LIMIT 1", new { user_safe }).FirstOrDefault();
            if (user_data == null)
            {
                ViewBag.Error = "Пользователя с таким ником не существует!";
                return View("error403");
            }
            data.ID = user_data.id;
            data.Username = user_data.username;
            data.Password = user_data.password_md5;
            data.Country = user_data.country;
            data.pRaw = user_data.privileges;
            data.Privileges = (UserPrivileges)data.pRaw;

            if (!BCrypt.Net.BCrypt.Verify(EncryptProvider.Md5(password).ToLowerInvariant(), data.Password))
            {
                ViewBag.Error = "Неверный пароль.";
                return View("error403");
            }

            var s = HttpContext.Session;
            if (UserPrivileges.Pending == data.Privileges)
            {
                Utility.setCookie(_db, HttpContext, data.ID);
                return RedirectToAction("Verify", "register", new { u = data.ID });
            }

            if (UserPrivileges.Banned == data.Privileges)
            {
                ViewBag.Error = "Ваш аккаунт был заблокирован/забанен. Обратитесь к Администрации.";
                return View("error403");
            }
            Utility.setCookie(_db, HttpContext, data.ID);
            s.SetInt32("userid", data.ID);
            s.SetString("pw", EncryptProvider.Md5(data.Password).ToLowerInvariant());
            s.CommitAsync();

            afterLogin(data.ID, HttpContext, data.Country);
            return RedirectToAction("Main", "main");
        }
        public void afterLogin(int user_id, HttpContext ctx, string country)
        {
            var s = GenerateToken(user_id);
            Utility.addCookie(ctx, "rt", EncryptProvider.Md5(s).ToLowerInvariant(), 24 * 30 * 1);
            if (country == "XX")
                Utility.setCountry(_db, HttpContext, user_id);
        }
        public string GenerateToken(int user_id)
        {
            var rs = Utility.GenerateString(32);
            _db.Query("tokens").Insert(new { user = user_id, privileges = '0', description = HttpContext.Connection.RemoteIpAddress.ToString(), token = EncryptProvider.Md5(rs).ToLowerInvariant(), last_updated = DateTime.UnixEpoch });
            return rs;
        }

        [HttpGet("/")]
        public IActionResult Main()
        {
            return View();
        }

        [HttpGet("switcher")]
        public IActionResult Switcher()
        {
            string file_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resources", "HimitsuSwitcher.exe");
            return PhysicalFile(file_path, System.Net.Mime.MediaTypeNames.Application.Octet, "HimitsuSwitcher.exe");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var ctx = HttpContext.Session;
            ctx.Clear();
            ctx.SetInt32("login", 0);
            ctx.CommitAsync();
            return RedirectToAction("Main", "main");
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            return View("error404");
        }

        [HttpGet("{*.}")]
        public IActionResult Error404()
        {
            HttpContext.Response.StatusCode = 404;
            return View("error404");
        }
    }
}
