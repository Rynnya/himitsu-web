using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Html;
using SqlKata.Execution;
using System.Linq;

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
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                {
                    return View("error404");
                }
                dynamic user = _db.Query("users").Select("username").Where("id", id).FirstOrDefault();
                if (user == null)
                    return View("error404");
                var http = new HttpClient();
                string data;
                try { data = http.GetStringAsync($"https://api.himitsu.ml/api/v1/users/full?id={id}").Result; }
                catch { return View("error404"); }
                var json = JToken.Parse(data);
                http = new HttpClient();
                data = http.GetStringAsync($"https://api.himitsu.ml/api/v1/users/full?id={id}&relax=1").Result;
                var relax = JToken.Parse(data);
                http = new HttpClient();
                data = http.GetStringAsync($"https://api.himitsu.ml/api/v1/users/userpage?id={id}").Result;
                IHtmlContentBuilder userpage = null;
                if (!string.IsNullOrEmpty(JToken.Parse(data)["userpage"].ToString()))
                    userpage = new HtmlContentBuilder().AppendHtml(Utility.ParseBB(JToken.Parse(data)["userpage"].ToString()));
                string[] bg = Utility.Background(_db, id);
                ViewBag.Background = bg[0];
                ViewBag.Horizontal = bg[1];
                ViewBag.Vertical = bg[2];
                ViewBag.Data = json;
                ViewBag.Userpage = userpage;
                ViewBag.Relax = relax;
                return View();
            }
            catch { return View("error500"); }
        }
    }
}
