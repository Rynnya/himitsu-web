using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
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
        public static void setCountry(MySqlConnection sql, HttpContext req, int user_id)
        {
            var url = $"https://ip.zxq.co/{req.Connection.RemoteIpAddress}/country";
            var raw = WebRequest.Create(url);
            var stream = new StreamReader(raw.GetResponse().GetResponseStream()).ReadToEnd();
            if (stream == "" || stream.Length != 2)
                return;
            var cmd = new MySqlCommand("UPDATE users_stats SET country = @stream WHERE id = @user_id", sql);
            cmd.Parameters.AddWithValue("@stream", stream);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            cmd.ExecuteNonQuery();
        }
        public static void LogIP(MySqlConnection sql, HttpContext req, int user_id) => new MySqlCommand($"INSERT INTO ip_user (userid, ip, occurencies) VALUES ({user_id}, {req.Connection.RemoteIpAddress}, '1') ON DUPLICATE KEY UPDATE occurencies = occurencies + 1", sql).ExecuteNonQuery();
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
        public static void setCookie(MySqlConnection sql, HttpContext req, int user_id)
        {
            string token;
            var cmd = new MySqlCommand($"SELECT token FROM identity_tokens WHERE userid = {user_id} LIMIT 1", sql);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                token = reader["token"].ToString();
                if (token != null)
                {
                    addCookie(req, "y", token);
                    reader.Close();
                    return;
                }
                else
                {
                    reader.Close();
                    while (true)
                    {
                        token = SHA256.Create().ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(GenerateString(30))).ToString();
                        reader = new MySqlCommand($"SELECT 1 FROM identity_tokens WHERE token = {token} LIMIT 1", sql).ExecuteReader();
                        if (reader.Read() && reader["1"] == null)
                        {
                            reader.Close();
                            break;
                        }
                        reader.Close();
                    }
                    cmd = new MySqlCommand("INSERT INTO identity_tokens(userid, token) VALUES (@userid, @token)", sql);
                    cmd.Parameters.AddWithValue("@userid", user_id);
                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.ExecuteNonQuery();
                    addCookie(req, "y", token);
                }
            }    
            else
                throw new Exception("Cannot read data from DB.");
        }
        private static void addCookie(HttpContext req, string name, string data)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(24 * 30 * 6)
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
