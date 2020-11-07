using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Q101.BbCodeNetCore;
using Microsoft.AspNetCore.Html;
using SqlKata.Execution;
using System.Linq;
using System;

namespace Himitsu.Pages
{
    public class profileController : Controller
    {
        private readonly QueryFactory _db;
        public profileController(QueryFactory db)
        {
            _db = db;
        }

        public IActionResult Profile(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                {
                    return View("error404");
                }
                string user = null;
                try { user = _db.Query("users").Select("username").Where("id", id).First().username; }
                catch {
                    if (HttpContext.Request.Cookies["redirect"] == null)
                    {
                        Console.WriteLine("WARN | ERR | MySQL panic! Username is null, so we return redirect to avoid error");
                        Utility.addCookie(HttpContext, "redirect", "1", 1);
                        return RedirectToAction("Profile", new { id });
                    }
                    HttpContext.Response.Cookies.Delete("redirect");
                    return View("error404");
                }
                if (user == null)
                    return View("error404");
                var http = new HttpClient();
                string data;
                try { data = http.GetStringAsync($"http://192.168.100.11:40001/api/v1/users/full?id={id}").Result; }
                catch { return View("error404"); }
                var json = JToken.Parse(data);
                http = new HttpClient();
                data = http.GetStringAsync($"http://192.168.100.11:40001/api/v1/users/full?id={id}&relax=1").Result;
                var relax = JToken.Parse(data);
                http = new HttpClient();
                data = http.GetStringAsync($"http://192.168.100.11:40001/api/v1/users/userpage?id={id}").Result;
                var bbcode = new BbCodeParser(Q101.BbCodeNetCore.Enums.ErrorMode.ErrorFree, null, new[]
                {
                    new BbTag("b", "<b>", "</b>", true, true),
                    new BbTag("url", "<a href=\"${href}\" class=\"bbcode-url\">", "</a>", new BbAttribute("href", ""), new BbAttribute("href", "href")),
                    new BbTag("img", "<img src=\"${content}\" class=\"bbcode-image\" />", "", false, true),
                    new BbTag("u", "<em class=\"bbcode-underline\">", "</em>"),
                    new BbTag("s", "<del class=\"bbcode-strikethrough\">", "</del>"),
                    new BbTag("i", "<em class=\"bbcode-italic\">", "</em>"),
                    new BbTag("code", "<code class=\"bbcode-code\">", "</code>"),
                    new BbTag("quote", "<blockquote class=\"bbcode-quote\">", "</blockquote>"),
                    new BbTag("sup", "<sup class=\"bbcode-superscript\">", "</sup>"),
                    new BbTag("sub", "<sub class=\"bbcode-subscript\">", "</sub>"),
                    new BbTag("center", "<center>", "</center>")
                });
                var userpage = new HtmlContentBuilder().AppendHtml(bbcode.ToHtml(JToken.Parse(data)["userpage"].ToString()));
                ViewBag.Data = json;
                ViewBag.Userpage = userpage;
                ViewBag.Relax = relax;
                return View();
            }
            catch { return View("error500"); }
        }
    }
}
