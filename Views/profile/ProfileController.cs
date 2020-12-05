using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Html;
using SqlKata.Execution;
using System.Linq;
using Himitsu.Models;
using Microsoft.AspNetCore.Http;

namespace Himitsu.Pages
{
    public class profileController : Controller
    {
        private readonly QueryFactory _db;
        public profileController(QueryFactory db)
        {
            _db = db;
        }

        public IActionResult Profile(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
                return View("error404");
            if (_db.Query("users").Select("username").Where("id", id).FirstOrDefault() == null)
                return View("error404");
            HttpClient http = new HttpClient();
            string data;
            try { data = http.GetStringAsync($"https://api.himitsu.ml/api/v1/users/full?id={id}&relax=-1").Result; }
            catch { return View("error404"); }
            JToken json = JToken.Parse(data);
            http = new HttpClient();
            data = http.GetStringAsync($"https://api.himitsu.ml/api/v1/users/userpage?id={id}").Result;
            IHtmlContentBuilder userpage = null;
            if (!string.IsNullOrEmpty(JToken.Parse(data)["userpage"].ToString()))
                userpage = new HtmlContentBuilder().AppendHtml(Utility.ParseBB(JToken.Parse(data)["userpage"].ToString()));
            Profile profile = new Profile(json)
            {
                ID = id,
                Userpage = userpage,
                Background = new Background(_db, (int)HttpContext.Session.GetInt32("userid"))
            };
            return View(profile);
        }
    }
}
