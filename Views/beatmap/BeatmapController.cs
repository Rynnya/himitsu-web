using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.JSInterop;
using MySql.Data.MySqlClient;
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
        public string data;
        private readonly QueryFactory _db;

        public beatmapController(QueryFactory db)
        {
            _db = db;
        }
        public IActionResult Beatmap(ulong? id)
        {
            if (string.IsNullOrEmpty(id.ToString())) return View("error404");
            try { ViewBag.Status = (int)_db.Query("beatmaps").Where("beatmap_id", id).Select("ranked").First().ranked; }
            catch { Console.WriteLine("WARN | ERR | MySQL panic! Beatmap ranking is null!"); ViewBag.Status = null; }

            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/b/{id}").Result; } catch { return View("error404"); }
            ViewBag.BeatmapID = JToken.Parse(data)["BeatmapID"];

            try { data = http.GetStringAsync($"http://storage.ripple.moe/api/s/{JToken.Parse(data)["ParentSetID"]}").Result; } catch { return View("error404"); }
            var token = JToken.Parse(data);

            ViewBag.HasVideo = Convert.ToBoolean(token["HasVideo"]);
            ViewBag.BeatmapSetID = token["SetID"];
            ViewBag.Title = token["Title"];
            ViewBag.Artist = token["Artist"];
            ViewBag.Creator = token["Creator"];
            var beatmap = JsonConvert.DeserializeObject<List<BeatmapData>>(JToken.Parse(data)["ChildrenBeatmaps"].ToString());
            var beatmapSort = beatmap.OrderBy(x => x.DifficultyRating);
            var set = JsonConvert.SerializeObject(beatmapSort);
            ViewBag.Set = new HtmlContentBuilder().AppendHtml(set);
            return View();
        }
    }
}
