let beatmapFull = true;
var mapset = []; 
function beatmapFullDiff() {
    var diffs = document.getElementById("beatmap-diffs");
    var button = document.getElementById("diff-button")


    if (beatmapFull == false) {
        button.textContent = button.textContent.replace('Скрыть', 'Показать все');
        diffs.style.maxWidth = "200px";
        diffs.style.maxHeight = "100px";
        beatmapFull = true;
    }

    else {
        button.textContent = button.textContent.replace('Показать все', 'Скрыть');
        diffs.style.maxWidth = "none";
        diffs.style.maxHeight = "none";
        beatmapFull = false;
    }
}

function difficultySelect(stars) {
    var circle = "";
    if (stars >= 0 && stars < 2) { circle = "easy"; }
    if (stars >= 2 && stars < 3) { circle = "normal"; }
    if (stars >= 3 && stars < 4) { circle = "hard"; }
    if (stars >= 4 && stars < 5) { circle = "insane"; }
    if (stars >= 5 && stars < 7) { circle = "extra"; }
    if (stars >= 7) { circle = "extra-plus"; }
    return circle;
}
function initUpdate(json) {
    $("div#beatmap-main-bg").css("background-image", `url(https://assets.ppy.sh/beatmaps/${beatmapSetID}/covers/cover@2x.jpg)`);
    var i = 0;
    json.forEach(function (w) { mapset[w.BeatmapID] = w; });
    $("text#beatmap-diff-name").html(`[${mapset[beatmapID].DifficultyRating.toFixed(2)} ★] ${mapset[beatmapID].DiffName.replace("'", "\'")}`);
    loadLeaderboard(beatmapID, 0, 0);
    loadDifficulties();
}

function loadLeaderboard(id, mode, relax) {
    $("tbody#leaderboard-user-data").empty();
    api("scores?sort=score,desc&sort=id,asc", { mode: mode, b: id, p: 1, l: 50, relax: relax }, function (data) {
        if (data.scores == null) { data.scores = []; }
        var leaderboard = "";
        var i = 0;
        data.scores.sort(function (a, b) { return b.score - a.score; });
        data.scores.forEach(function (score) {
            var user = score.user;
            leaderboard += `<tr id="leaderboard-user-data" class="text-left">`;
            leaderboard += `<td class="text-center">#${++i}</td>`;
            leaderboard += `<td class="text-center">${score.rank}</td>`;
            leaderboard += `<td><img src="../resources/flags/${user.country}.png" width="30px" height="20px"/></td>`;
            leaderboard += `<td><a href="${'/u/' + user.id}">${user.username}</a></td>`;
            leaderboard += `<td>${addCommas(score.max_combo)}</td>`;
            leaderboard += `<td>${score.pp.toFixed()}</td>`;
            leaderboard += `<td>${addCommas(score.score)}</td>`;
            leaderboard += `<td>${score.accuracy.toFixed(2)}%</td>`;
            leaderboard += `<td>${modbits.toString(score.mods)}</td>`;
            leaderboard += `<td>${score.count_300 + score.count_geki}</td>`;
            leaderboard += `<td>${score.count_100 + score.count_katu}</td>`;
            leaderboard += `<td>${score.count_50}</td>`;
            leaderboard += `<td>${score.count_miss}</td></tr>`;
        });
        $("tbody#leaderboard-user-data").append(leaderboard);
    });
    curr_mode = mode;
    curr_rx = relax;
}

function loadDifficulties() {
    var diff = "";
    if (jsonData.length < 15) {
        $("div#diff-button.show-all-diffs").css("display", "none");
    }
    jsonData.forEach(function (score) {
        diff += `<a href="/b/${score.BeatmapID}"><div onmouseover="changeDiffName(${score.DifficultyRating.toFixed(2)}, '${score.DiffName.replace("'", "\\\'")}')" class="beatmap-diff"><img src="../resources/beatmap/circle-${difficultySelect(score.DifficultyRating)}.png" height="25px" /></a></div>`;
    });
    $("div#beatmap-diffs").append(diff);
    $("td#cs").html(mapset[beatmapID].CS);
    $("td#ar").html(mapset[beatmapID].AR);
    $("td#od").html(mapset[beatmapID].OD);
    $("td#hp").html(mapset[beatmapID].HP);
    $("td#star").html(mapset[beatmapID].DifficultyRating.toFixed(2));
    $("td#time").html(timeFormat(mapset[beatmapID].TotalLength));
    $("td#bpm").html(mapset[beatmapID].BPM);
}

function changeDiffName(diff, name) {
    $("text#beatmap-diff-name").html(`[${diff} ★] ${name}`);
}