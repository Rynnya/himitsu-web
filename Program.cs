using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NETCore.Encrypt;
using Q101.BbCodeNetCore;
using SqlKata.Execution;

namespace Himitsu
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public enum UserPrivileges:long
    {
        Banned = 0,
        Restricted = 2,
        Normal_User = 3,
        BAT = 267,
        Beatmap_Nominator = 267,
        New_Privilege_Group = 33039,
        Community_Manager = 918015,
        Developer = 1043995,
        Admin = 1043995,
        God = 1043995,
        Pending = 1048576,
        Chat_Moderators = 2883911,
        Full_Perms = 3145727
    }
    public class Utility
    {
        public static string ParseBB(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;
            BbCodeParser bbcode = new BbCodeParser(Q101.BbCodeNetCore.Enums.ErrorMode.ErrorFree, null, new[]
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
                    new BbTag("center", "<center>", "</center>"),
                    new BbTag("color", "<span style=\"color: ${color};\">", "</span>", new BbAttribute("color", ""), new BbAttribute("color", "color")),
                    new BbTag("size", "<span style=\"font-size: ${size};\">", "</span>", new BbAttribute("size", ""), new BbAttribute("size", "size"))
                });
            return bbcode.ToHtml(code);
        }
        public static bool CheckPrivileges(HttpContext ctx, QueryFactory _db, UserPrivileges privileges)
        {
            if (!ctx.Session.Keys.Contains("userid"))
                return true;
            long priv = _db.Query("users").Select("privileges").Where("id", ctx.Session.GetInt32("userid")).First().privileges;
            if (priv < Convert.ToInt64(privileges) && priv != Convert.ToInt64(UserPrivileges.Pending))
                return true;
            return false;
        }
        public static bool EscapeDirectories(HttpRequest req)
        {
            if (req.Path.StartsWithSegments("/resources") || req.Path.StartsWithSegments("/css") || req.Path.StartsWithSegments("/js") || req.Path.StartsWithSegments("/favicon.ico"))
                return false;
            return true;
        }
        public static void setCountry(QueryFactory db, HttpContext req, int user_id)
        {
            string url = $"https://ip.zxq.co/{req.Connection.RemoteIpAddress}/country";
            WebRequest raw = WebRequest.Create(url);
            string stream = new StreamReader(raw.GetResponse().GetResponseStream()).ReadToEnd();
            if (stream == "" || stream.Length != 2)
                return;
            db.Query("users_stats").Update(new { country = stream.ToString(), id = user_id });
        }
        public static void setCookie(QueryFactory db, HttpContext req, int user_id)
        {
            dynamic token = db.Query("identity_tokens").Select("token").Where("userid", user_id).FirstOrDefault().token;
            if (token != null)
            {
                addCookie(req, "y", token, 24 * 30 * 6);
                return;
            }
            else
            {
                while (true)
                {
                    token = EncryptProvider.Sha256(GenerateString(30));
                    dynamic check = db.Query("identity_tokens").Select("token").Where("token", token).FirstOrDefault().token;
                    if (check == null)
                        break;
                }
                db.Query("identity_tokens").Insert(new { userid = user_id, token });
                addCookie(req, "y", token, 24 * 30 * 6);
            }
        }
        public static void addCookie(HttpContext req, string name, string data, int time)
        {
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(time)
            };
            req.Response.Cookies.Append(name, data, cookieOptions);
        }
        public static string GenerateString(int length = 15)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = validChars[random.Next(0, validChars.Length)];
            return new string(chars);
        }
        public static bool IntToBool(int i) => i == 1;
        public static bool StringToBool(string i) => i == "1" || i == "True" || i == "true";
    }
}
