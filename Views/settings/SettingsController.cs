﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System.Drawing.Imaging;
using System;
using System.Linq;
using System.Drawing;
using Microsoft.AspNetCore.Html;
using Himitsu.Models;
using NETCore.Encrypt;

namespace Himitsu.Views.settings
{
    [Route("[controller]")]
    public class settingsController : Controller
    {
        private readonly QueryFactory _db;

        public settingsController(QueryFactory db)
        {
            _db = db;
        }

        [HttpGet()]
        public IActionResult General()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            return Redirect("/settings/profile");
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            ViewBag.DB = _db.Query("users_stats").Select("play_style", "favourite_mode", "favourite_relax").Where("id", HttpContext.Session.GetInt32("userid")).FirstOrDefault();
            return View(model);
        }

        [HttpPost("profile")]
        public IActionResult Profile(int style, int mode, int relax)
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            ViewBag.DB = _db.Query("users_stats").Select("play_style", "favourite_mode", "favourite_relax").Where("id", HttpContext.Session.GetInt32("userid")).First();
            if (style > 15 || style < 0)
                return Redirect("/settings/profile");
            _db.Query("users_stats").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { play_style = (short)style, favourite_mode = mode, favourite_relax = relax });
            ViewBag.Success = "Настройки успешно изменены!";
            return View(model);
        }

        [HttpGet("avatar")]
        public IActionResult Avatar()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View(model);
        }

        [HttpPost("avatar")]
        public IActionResult Avatar(IFormFile avatar)
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Image img = Image.FromStream(avatar.OpenReadStream());
            Bitmap newimg = new Bitmap(img, new Size { Height = 256, Width = 256 });
            newimg.Save($"/home/pi/himitsu/avatar-server/Avatars/{HttpContext.Session.GetInt32("userid")}.png", ImageFormat.Png);
            return Redirect("/settings/avatar");
        }

        [HttpGet("userpage")]
        public IActionResult Userpage()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            ViewBag.Userpage = (string)_db.Query("users_stats").Select("userpage_content").Where("id", HttpContext.Session.GetInt32("userid")).FirstOrDefault().userpage_content;
            IHtmlContentBuilder userpage = new HtmlContentBuilder().AppendHtml(Utility.ParseBB((string)_db.Query("users_stats").Select("userpage_content").Where("id", HttpContext.Session.GetInt32("userid")).FirstOrDefault().userpage_content));
            if (userpage != null)
                ViewBag.Encoded = userpage;
            return View(model);
        }

        [HttpPost("userpage")]
        public IActionResult Userpage(string new_page)
        {
            _db.Query("users_stats").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { userpage_content = new_page });
            return Redirect("/settings/userpage");
        }

        [HttpPost("userpage/parse")]
        public string ParseBB(string code) => Utility.ParseBB(code);

        [HttpGet("background")]
        public IActionResult Background()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View(model);
        }

        [HttpPost("background")]
        public IActionResult Background(string back, int horiz, int vert)
        {
            _db.Query("users_stats").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { background_site = back, horizontal = horiz, vertical = vert });
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View(model);
        }

        [HttpGet("scoreboard")]
        public IActionResult Scoreboard()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Scoreboard score = new Scoreboard(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View(score);
        }

        [HttpPost("scoreboard")]
        public IActionResult Scoreboard(int submode, int vanilla_order, int relax_order, int std_prior, int taiko_prior, int ctb_prior, int mania_prior)
        {
            _db.Query("users").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { is_relax = (byte)submode });
            // fuck this whole life
            _db.Query("users_preferences").Where("id", HttpContext.Session.GetInt32("userid")).Update(new
            {
                scoreboard_display_classic = (byte)vanilla_order,
                scoreboard_display_relax = (byte)relax_order,
                score_overwrite_std = (byte)std_prior,
                score_overwrite_taiko = (byte)taiko_prior,
                score_overwrite_ctb = (byte)ctb_prior,
                score_overwrite_mania = (byte)mania_prior
            });
            return Redirect("/settings/scoreboard");
        }

        [HttpGet("password")]
        public IActionResult Password()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View(model);
        }

        [HttpPost("password")]
        public IActionResult Password(string old_pass, string new_pass, string check)
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            Background model = new Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            if (!BCrypt.Net.BCrypt.Verify(EncryptProvider.Md5(old_pass).ToLowerInvariant(), _db.Query("users").Select("password_md5").Where("id", HttpContext.Session.GetInt32("userid")).First().password_md5))
            {
                ViewBag.QuickError = "Неправильный пароль.";
                return View(model);
            }
            if (old_pass == new_pass)
            {
                ViewBag.QuickError = "Почему? Зачем?";
                return View(model);
            }
            if (new_pass != check)
            {
                ViewBag.QuickError = "Вы ввели два разных пароля.";
                return View(model);
            }
            _db.Query("users").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { password_md5 = BCrypt.Net.BCrypt.HashPassword(EncryptProvider.Md5(new_pass)) });
            HttpContext.Session.SetString("pw", BCrypt.Net.BCrypt.HashPassword(EncryptProvider.Md5(new_pass)));
            return Redirect("/settings/password");
        }
    }
}
