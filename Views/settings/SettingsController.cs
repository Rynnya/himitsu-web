using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System;
using System.Linq;

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
            ViewBag.DB = _db.Query("users_stats").Select("play_style", "favourite_mode", "favourite_relax").Where("id", HttpContext.Session.GetInt32("userid")).First();
            return View();
        }

        [HttpPost("profile")]
        public IActionResult Profile(int style, int mode, int relax)
        {
            if (style > 15 || style < 0)
                return Redirect("/settings/profile");
            _db.Query("users_stats").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { play_style = style, favourite_mode = mode, favourite_relax = relax });
            return Redirect("/settings/profile");
        }
        [HttpGet("userpage")]
        public IActionResult Userpage()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            return View("error404");
        }

        [HttpGet("avatar")]
        public IActionResult Avatar()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            return View("error404");
        }

        [HttpGet("scoreboard")]
        public IActionResult Scoreboard()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            return View("error404");
        }

        [HttpGet("password")]
        public IActionResult Password()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            return View("error404");
        }
    }
}
