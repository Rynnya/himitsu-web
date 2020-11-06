using Microsoft.AspNetCore.Mvc;

namespace Himitsu.Views.settings
{
    [Route("[controller]")]
    public class settingsController : Controller
    {
        [HttpGet()]
        public IActionResult General()
        {
            return Redirect("/settings/profile");
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return View("error404");
        }

        [HttpGet("userpage")]
        public IActionResult Userpage()
        {
            return View("error404");
        }

        [HttpGet("avatar")]
        public IActionResult Avatar()
        {
            return View("error404");
        }

        [HttpGet("scoreboard")]
        public IActionResult Scoreboard()
        {
            return View("error404");
        }

        [HttpGet("password")]
        public IActionResult Password()
        {
            return View("error404");
        }
    }
}
