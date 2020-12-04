using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlKata.Execution;
using Himitsu.Models;
using System;

namespace Himitsu.Views.beatmap
{
    public class BeatmapData
    {
        public ulong BeatmapID;
        public ulong ParentSetID;
        public string DiffName;
        public string FileMD5;
        public int Mode;
        public double BPM;
        public double AR;
        public double OD;
        public double CS;
        public double HP;
        public int TotalLength;
        public int HitLength;
        public ulong Playcount;
        public ulong Passcount;
        public int MaxCombo;
        public double DifficultyRating;
    }
    public class beatmapController : Controller
    {
        public HttpClient http = new HttpClient();
        private readonly QueryFactory _db;

        public beatmapController(QueryFactory db)
        {
            _db = db;
        }
        public IActionResult Beatmap(ulong id)
        {
            if (string.IsNullOrEmpty(id.ToString())) return View("error404");
            string data;
            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/b/{id}").Result; } catch { return View("error404"); }
            JToken token = JToken.Parse(data);
            ViewBag.Beatmap = token;

            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/s/{token["ParentSetID"]}").Result; } catch { return View("error404"); }
            JToken token2 = JToken.Parse(data);

            Beatmap model = new Beatmap(_db, id, token2) {
                Mode = (Convert.ToInt32(token["Mode"])) switch
                {
                    0 => "osu!",
                    1 => "osu!taiko",
                    2 => "osu!ctb",
                    3 => "osu!mania",
                    _ => "osu!",
                }
            };
            return View(model);
        }

        public IActionResult Set(ulong id)
        {
            if (string.IsNullOrEmpty(id.ToString())) return View("error404");
            string data;
            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/s/{id}").Result; } catch { return View("error404"); }
            JToken token = JToken.Parse(data);

            Beatmap model = new Beatmap(_db, id, token);
            return View("beatmap", model);
        }
    }
}
