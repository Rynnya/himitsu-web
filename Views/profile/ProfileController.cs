using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Q101.BbCodeNetCore;
using Microsoft.AspNetCore.Html;

namespace Himitsu.Pages
{
    public class profileController : Controller
    {
        private readonly MySqlConnection _sql;
        private MySqlDataReader reader;
        private MySqlCommand cmd;
        public profileController(MySqlConnection sql)
        {
            _sql = sql;
        }

        public ViewResult Profile(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                {
                    return View("error404");
                }
                cmd = new MySqlCommand("SELECT username FROM users WHERE id = @id LIMIT 1", _sql);
                cmd.Parameters.AddWithValue("@id", id);
                reader = cmd.ExecuteReader();
                if (reader.Read() && reader["username"] == null)
                {
                    reader.Close();
                    return View("error404");
                }
                reader.Close();
                var http = new HttpClient();
                var data = http.GetStringAsync($"http://192.168.100.11:40001/api/v1/users/full?id={id}").Result;
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
