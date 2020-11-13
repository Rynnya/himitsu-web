using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using System.Drawing.Imaging;
using System;
using System.Linq;
using System.Drawing;
using Microsoft.AspNetCore.Html;

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
            ViewBag.Background = Utility.Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            ViewBag.DB = _db.Query("users_stats").Select("play_style", "favourite_mode", "favourite_relax").Where("id", HttpContext.Session.GetInt32("userid")).First();
            return View();
        }

        [HttpPost("profile")]
        public IActionResult Profile(int style, int mode, int relax)
        {
            if (style > 15 || style < 0)
                return Redirect("/settings/profile");
            if (HttpContext.Session.Keys.Contains("userid"))
                _db.Query("users_stats").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { play_style = style, favourite_mode = mode, favourite_relax = relax });
            return Redirect("/settings/profile");
        }

        [HttpGet("avatar")]
        public IActionResult Avatar()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            ViewBag.Background = Utility.Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View();
        }

        [HttpPost("avatar")]
        public IActionResult Avatar(IFormFile avatar)
        {
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
            ViewBag.Background = Utility.Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            ViewBag.Userpage = (string)_db.Query("users_stats").Select("userpage_content").Where("id", HttpContext.Session.GetInt32("userid")).First().userpage_content;
            try { ViewBag.Encoded = new HtmlContentBuilder().AppendHtml(Utility.ParseBB((string)_db.Query("users_stats").Select("userpage_content").Where("id", HttpContext.Session.GetInt32("userid")).First().userpage_content)); }
            catch { ViewBag.Encoded = null; }
            return View();
        }

        [HttpPost("userpage")]
        public IActionResult Userpage(string new_page)
        {
            if (HttpContext.Session.Keys.Contains("userid"))
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
            string done = _db.Query("users_stats").Select("background_site").Where("id", HttpContext.Session.GetInt32("userid")).First().background_site;
            if (string.IsNullOrWhiteSpace(done))
                ViewBag.Background = "https://i.pinimg.com/originals/f1/63/11/f16311fd0c32786525f471c685bc516e.gif";
            else
                ViewBag.Background = done;
            return View();
        }

        [HttpPost("background")]
        public IActionResult Background(string back)
        {
            if (HttpContext.Session.Keys.Contains("userid"))
                _db.Query("users_stats").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { background_site = back });
            ViewBag.Background = back;
            return View();
        }

        [HttpGet("scoreboard")]
        public IActionResult Scoreboard()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            ViewBag.r = (int)_db.Query("users").Select("is_relax").Where("id", HttpContext.Session.GetInt32("userid")).First().is_relax;
            ViewBag.q = _db.Query("users_preferences").Where("id", HttpContext.Session.GetInt32("userid")).First();
            ViewBag.Background = Utility.Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View();
        }

        [HttpPost("scoreboard")]
        public IActionResult Scoreboard(int submode, int vanilla_order, int relax_order, int std_prior, int taiko_prior, int ctb_prior, int mania_prior)
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            _db.Query("users").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { is_relax = submode });
            // fuck this whole life
            _db.Query("users_preferences").Where("id", HttpContext.Session.GetInt32("userid")).Update(new
            {
                scoreboard_display_classic = vanilla_order,
                scoreboard_display_relax = relax_order,
                score_overwrite_std = std_prior,
                score_overwrite_taiko = taiko_prior,
                score_overwrite_ctb = ctb_prior,
                score_overwrite_mania = mania_prior
            });
            return Redirect("/settings/scoreboard");
        }

        [HttpGet("password")]
        public IActionResult Password()
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            ViewBag.Background = Utility.Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            return View();
        }

        [HttpPost("password")]
        public IActionResult Password(string old_pass, string new_pass, string check)
        {
            if (!HttpContext.Session.Keys.Contains("userid"))
                return RedirectToAction("login", "main");
            ViewBag.Background = Utility.Background(_db, (int)HttpContext.Session.GetInt32("userid"));
            if (!BCrypt.Net.BCrypt.Verify(Utility.CreateMD5(old_pass).ToLowerInvariant(), HttpContext.Session.GetString("pw")))
            {
                ViewBag.QuickError = "Неправильный пароль.";
                return View();
            }
            if (old_pass == new_pass)
            {
                ViewBag.QuickError = "Почему? Зачем?";   
                return View();
            }
            if (new_pass != check)
            {
                ViewBag.QuickError = "Вы ввели два разных пароля.";
                return View();
            }
            _db.Query("users").Where("id", HttpContext.Session.GetInt32("userid")).Update(new { password_md5 = BCrypt.Net.BCrypt.HashPassword(Utility.CreateMD5(new_pass)) });
            HttpContext.Session.SetString("pw", BCrypt.Net.BCrypt.HashPassword(Utility.CreateMD5(new_pass)));
            return Redirect("/settings/password");
        }
    }
}
