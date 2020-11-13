using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Q101.BbCodeNetCore;
using SqlKata.Execution;
using SshNet.Security.Cryptography;

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

    public enum UserPrivileges
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
        public static string Background(QueryFactory db, int id)
        {
            try
            {
                string background = db.Query("users_stats").Select("background_site").Where("id", id).First().background_site;
                if (string.IsNullOrEmpty(background))
                    background = "https://i.pinimg.com/originals/f1/63/11/f16311fd0c32786525f471c685bc516e.gif";
                return background;
            }
            catch { return "https://i.pinimg.com/originals/f1/63/11/f16311fd0c32786525f471c685bc516e.gif"; }
        }
        public static string ParseBB(string code)
        {
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
            return bbcode.ToHtml(code);
        }
        public static bool CheckPrivileges(HttpContext ctx, QueryFactory _db, UserPrivileges privileges)
        {
            if (!ctx.Session.Keys.Contains("userid"))
                return true;
            long priv = _db.Query("users").Select("privileges").Where("id", ctx.Session.GetInt32("userid")).First().id;
            if (priv < (long)privileges)
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
            var url = $"https://ip.zxq.co/{req.Connection.RemoteIpAddress}/country";
            var raw = WebRequest.Create(url);
            var stream = new StreamReader(raw.GetResponse().GetResponseStream()).ReadToEnd();
            if (stream == "" || stream.Length != 2)
                return;
            db.Query("users_stats").Update(new { country = stream.ToString(), id = user_id });
        }
        public static void LogIP(QueryFactory db, HttpContext req, int user_id)
        {
            try { db.Statement("INSERT INTO ip_user (userid, ip, occurencies) VALUES ({@userid}, {@ip}, '1') ON DUPLICATE KEY UPDATE occurencies = occurencies + 1", new { userid = user_id, ip = req.Connection.RemoteIpAddress.ToString() }); }
            catch { Console.WriteLine($"WARN | 500 | Cannot resolve IP address of {user_id}"); }
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static void setCookie(QueryFactory db, HttpContext req, int user_id)
        {
            string token;
            try { token = db.Query("identity_tokens").Select("token").Where("userid", user_id).First().token; }
            catch { token = null; }
            if (token != null)
            {
                addCookie(req, "y", token, 24 * 30 * 6);
                return;
            }
            else
            {
                while (true)
                {
                    token = SHA256.Create().ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(GenerateString(30))).ToString();
                    try { _ = db.Query("identity_tokens").Select("token").Where("token", token).First().token; }
                    catch { break; }
                }
                db.Query("identity_tokens").Insert(new { userid = user_id, token });
                addCookie(req, "y", token, 24 * 30 * 6);
            }    
        }
        public static void addCookie(HttpContext req, string name, string data, int time)
        {
            var cookieOptions = new CookieOptions
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
