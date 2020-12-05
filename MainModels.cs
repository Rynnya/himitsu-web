using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Himitsu.Models
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
    public class Beatmap
    {
        public string Rank { get; set; }
        public bool hasVideo { get; set; }
        public int BeatmapSetID { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Creator { get; set; }
        public IHtmlContentBuilder HtmlSet { get; set; }
        public string Mode { get; set; }
        public Beatmap(QueryFactory _db, ulong id, JToken token)
        {
            hasVideo = Convert.ToBoolean(token["HasVideo"]);
            BeatmapSetID = Convert.ToInt32(token["SetID"]);
            Title = token["Title"].ToString();
            Artist = token["Artist"].ToString();
            Creator = token["Creator"].ToString();
            List<BeatmapData> beatmap = JsonConvert.DeserializeObject<List<BeatmapData>>(token["ChildrenBeatmaps"].ToString());
            HtmlSet = new HtmlContentBuilder().AppendHtml(JsonConvert.SerializeObject(beatmap.OrderBy(x => x.DifficultyRating)));
            int status = 0;
            dynamic check =_db.Query("beatmaps").Where("beatmapset_id", id).Select("ranked").FirstOrDefault();
            if (check != null)
                status = check.ranked;
            Rank = status switch
            {
                -2 => "Unknown",
                -1 => "Not Submitted",
                1 => "Need Update",
                2 => "Ranked",
                3 => "Approved",
                4 => "Qualified",
                5 => "Loved",
                _ => "Unranked",
            };
        }
    }
    public class Profile
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Country { get; set; }
        public int PlayStyle { get; set; }
        public int FavClassic { get; set; }
        public int FavRelax { get; set; }
        public JToken Classic { get; set; }
        public JToken Relax { get; set; }
        public IHtmlContentBuilder Userpage { get; set; }
        public Background Background { get; set; }
        public Profile(JToken json)
        {
            Username = json["username"].ToString();
            Country = json["country"].ToString();
            PlayStyle = Convert.ToInt32(json["play_style"]);
            FavClassic = Convert.ToInt32(json["favourite_mode"]);
            FavRelax = Convert.ToInt32(json["favourite_relax"]);
            Classic = json["stats"]["classic"];
            Relax = json["stats"]["relax"];
        }
    }

    // Main class for Background on any page
    public class Background
    {
        public string Link { get; set; }
        public string Horizontal { get; set; }
        public string Vertical { get; set; }
        public Background(QueryFactory _db, int id)
        {
            dynamic done = _db.Query("users_stats").Select("background_site", "horizontal", "vertical").Where("id", id).FirstOrDefault();
            if (done == null)
            {
                Link = "https://i.pinimg.com/originals/f1/63/11/f16311fd0c32786525f471c685bc516e.gif";
                Horizontal = "0%"; Vertical = "0%";
                return;
            }
            Link = done.background_site;
            Horizontal = done.horizontal + "%";
            Vertical = done.vertical + "%";
        }
    }

    public class Scoreboard
    {
        public int isRelax { get; set; }
        public int ScoreClassic { get; set; }
        public int ScoreRelax { get; set; }
        public int ScoreOW_STD { get; set; }
        public int ScoreOW_Taiko { get; set; }
        public int ScoreOW_CTB { get; set; }
        public int ScoreOW_Mania { get; set; }
        public Background Background { get; set; }
        public Scoreboard(QueryFactory _db, int id)
        {
            isRelax = _db.Query("users").Select("is_relax").Where("id", id).First().is_relax;
            dynamic score = _db.Query("users_preferences").Where("id", id).First();
            ScoreClassic = score.scoreboard_display_classic;
            ScoreRelax = score.scoreboard_display_relax;
            ScoreOW_STD = score.score_overwrite_std;
            ScoreOW_Taiko = score.score_overwrite_taiko;
            ScoreOW_CTB = score.score_overwrite_ctb;
            ScoreOW_Mania = score.score_overwrite_mania;
            Background = new Background(_db, id);
        }
    }
}
