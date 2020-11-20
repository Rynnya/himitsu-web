using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlKata.Execution;

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
            dynamic rank = _db.Query("beatmaps").Where("beatmap_id", id).Select("ranked").FirstOrDefault().ranked;
            if (rank != null)
                rank = rank.ranked;

            string data;
            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/b/{id}").Result; } catch { return View("error404"); }
            var token = JToken.Parse(data);
            ViewBag.Beatmap = token;
            ViewBag.Mode = token["Mode"];

            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/s/{token["ParentSetID"]}").Result; } catch { return View("error404"); }
            token = JToken.Parse(data);

            ViewBag.Status = rank;
            ViewBag.HasVideo = Convert.ToBoolean(token["HasVideo"]);
            ViewBag.BeatmapSetID = token["SetID"];
            ViewBag.Title = token["Title"];
            ViewBag.Artist = token["Artist"];
            ViewBag.Creator = token["Creator"];
            var beatmap = JsonConvert.DeserializeObject<List<BeatmapData>>(token["ChildrenBeatmaps"].ToString());
            var beatmapSort = beatmap.OrderBy(x => x.DifficultyRating);
            var set = JsonConvert.SerializeObject(beatmapSort);
            ViewBag.Set = new HtmlContentBuilder().AppendHtml(set);
            return View();
        }

        public IActionResult Set(ulong id)
        {
            if (string.IsNullOrEmpty(id.ToString())) return View("error404");
            dynamic rank = _db.Query("beatmaps").Where("beatmapset_id", id).Select("ranked").FirstOrDefault().ranked;
            if (rank != null)
                rank = rank.ranked;

            string data;
            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/s/{id}").Result; } catch { return View("error404"); }
            var token = JToken.Parse(data);

            ViewBag.Status = rank;
            ViewBag.HasVideo = Convert.ToBoolean(token["HasVideo"]);
            ViewBag.BeatmapSetID = token["SetID"];
            ViewBag.Title = token["Title"];
            ViewBag.Artist = token["Artist"];
            ViewBag.Creator = token["Creator"];
            ViewBag.Beatmap = JToken.Parse(data)["ChildrenBeatmaps"][0];
            ViewBag.Mode = JToken.Parse(data)["ChildrenBeatmaps"][0]["Mode"];
            var beatmap = JsonConvert.DeserializeObject<List<BeatmapData>>(JToken.Parse(data)["ChildrenBeatmaps"].ToString());
            var beatmapSort = beatmap.OrderBy(x => x.DifficultyRating);
            var set = JsonConvert.SerializeObject(beatmapSort);
            ViewBag.Set = new HtmlContentBuilder().AppendHtml(set);
            return View("beatmap");
        }
    }
}
