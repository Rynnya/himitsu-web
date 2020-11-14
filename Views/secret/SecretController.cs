using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;

namespace Himitsu.Views.secret
{
    [Route("[controller]")]
    public class secretController : Controller
    {
        private readonly QueryFactory _db;
        public secretController(QueryFactory db)
        {
            _db = db;
        }

        [HttpGet("ranking")]
        public IActionResult Ranking()
        {
            if (Utility.CheckPrivileges(HttpContext, _db, UserPrivileges.BAT))
                return View("error404");
            return View();
        }

        [HttpPost("ranking")]
        public IActionResult Ranking(string map, string type)
        {
            if (Utility.CheckPrivileges(HttpContext, _db, UserPrivileges.BAT))
                return View("error404");
            if (string.IsNullOrEmpty(map))
            {
                ViewBag.QuickError = "Вы не ввели ссылку на карту!";
                return View();
            }
            int rank;
            switch (type.ToLowerInvariant())
            {
                case "ranked":
                    rank = 2;
                    break;
                case "loved":
                    rank = 5;
                    break;
                case "derank":
                    rank = 0;
                    break;
                default:
                    rank = 0;
                    break;
            }
            Regex reg = new Regex(@"^https?:\/\/osu.ppy.sh\/(s|b)\/(\d+)$");
            if (!reg.IsMatch(map))
            {
                ViewBag.QuickError = "Вы должны ввести ссылку типа https://osu.ppy.sh/b/xxxx";
                return View();
            }
            string preready = map.Substring(map.LastIndexOf("/") - 1);
            string id = preready.Substring(2, preready.Length - 2);
            string beatmap_ctx;
            if (preready[0] == 'b')
                beatmap_ctx = "beatmap_id";
            else
                beatmap_ctx = "beatmapset_id";
            _db.Query("beatmaps").Where(beatmap_ctx, id).Update(new { ranked = (byte)rank });
            _db.Query("ranking_logs").Insert(new { map_id = id, modified_by = HttpContext.Session.GetInt32("userid"), ranked = (byte)rank, map_type = beatmap_ctx });
            return View();
        }
    }

    [Route("[controller]")]
    public class adminController : Controller
    {
        private readonly QueryFactory _db;
        public adminController(QueryFactory db)
        {
            _db = db;
        }

        [HttpGet("useredit")]
        public IActionResult UserEdit(int id)
        {
            return View("error404"); // world not ready to see this thing
            if (string.IsNullOrEmpty(id.ToString()))
                return View("error404");
            if (Utility.CheckPrivileges(HttpContext, _db, UserPrivileges.Admin))
                return View("error404");
            return View();
        }
    }
}
