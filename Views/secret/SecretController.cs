using System;
using System.Linq;
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
            return View("error404"); // world not ready to see this thing
            if (Utility.CheckPrivileges(HttpContext, _db, UserPrivileges.BAT))
                return View("error404");
            return View();
        }

        [HttpPost("ranking")]
        public IActionResult Ranking(int map_id, int type)
        {
            return View("error404"); // world not ready to see this thing
            if (Utility.CheckPrivileges(HttpContext, _db, UserPrivileges.BAT))
                return View("error404");
            if (type > 5 || type < 0)
            {
                ViewBag.Error = "Вы ввели невозможное значение ранкинга!";
                return View("error403");
            }
            _db.Query("beatmaps").Where("beatmap_id", map_id).Update(new { ranked = type });
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
