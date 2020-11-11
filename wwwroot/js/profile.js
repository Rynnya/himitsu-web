var id = 0; var page = 1;
var curr_mode = 0; var curr_rx = 0; var play_mode = "best";

function displayContent(){
  document.getElementById('content').style.display = "flex";
}

const mods = [
    "std",
    "taiko",
    "ctb",
    "mania"
]
let mod = "";
let profile = 0;
const modNameSuffix = ["", "-relax"]

function profileMode(modId) {
    mod = modNameSuffix[modId];
    profileGameMode(profile);
}

function profileGameMode(menuId) {
    profile = menuId;
    const modSuffix = "-" + mods[menuId] + mod;
    ShowOneChild("profile-all-tables", "profile-table" + modSuffix);
    ShowOneChild("profile-levels", "profile-level" + modSuffix);
}

function initJS(num, relax, mod) {
    id = num;
    profileMode(mod);
    osuMode(relax);
    osuGameMode(mod);
    updateScores(mod);
    profileMaps(0);
}

function profileMaps(bottomMaps) {
    play_mode = bottomMaps == 1 ? 'recent' : 'best';
    updateScores(curr_mode);

    let bottomMenuButton = [document.getElementById("profile-scores-best"), document.getElementById("profile-scores-last")]

    for (let i of bottomMenuButton) {
        i.style.color = "#888888";
        i.style.borderBottom = "none";
    }

    if (bottomMaps >= 0 && bottomMaps <= 1) {
        bottomMenuButton[bottomMaps].style.borderBottom = "solid 3px black";
        bottomMenuButton[bottomMaps].style.color = "black";
    }
}

let profileFull = false;
function profileFullBg() {
    var profileBg = document.getElementById("profile-main-background");
    var profile = document.getElementById("profile-tables");
    var avatar = document.getElementById("avatar");

    if (profileFull == false) {
        avatar.style.opacity = "0.2";
        profile.style.opacity = "0";
        profileBg.style.minHeight = "500px";
        profileFull = true;
    }

    else {
        avatar.style.opacity = "1";
        profile.style.opacity = "1";
        profileBg.style.minHeight = "0px";
        profileFull = false;
    }
}

let beatmapFull = true;
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

function SetHasClass(panel, className, bool) {
    const isHasClass = panel.classList.contains(className);
    if (isHasClass) {
        if (!bool) panel.classList.remove(className);
    } else {
        if (bool) panel.classList.add(className);
    }
}

function HideAllChildren(panel) {
    for (child of panel.children) {
        SetHasClass(child, "hide", true);
    }
}
function ShowOneChild(parentPanel, childName) {
    HideAllChildren(document.getElementById(parentPanel))
    SetHasClass(document.getElementById(childName), "hide", false);
}


function updateRelax(relax) {
    curr_rx = relax;
    updateScores(curr_mode);
}
function updateScores(modID) {
    $("#scores-data").empty();
    api('users/scores/' + play_mode, { id: id, p: 1, l: 10, mode: modID, relax: curr_rx }, function (data) {
        var scores = 0;
        var new_scores = "";
        data["scores"].forEach(element => {
            var mods = modbits.toString(element["mods"]).match(/.{1,2}/g);
            var mods_names = modnames.toString(mods).split('/');
            new_scores += `<div class="score white-text" style="background-image: url(../resources/profile/rank_bg/rank_${element["rank"]}.png)">`;
            new_scores += `<img src="../resources/profile/rank_rank/${element["rank"]}_nobg.png" height="50px" />`;
            new_scores += `<div class="score-info"><a class="link-button" href="/b/${element["beatmap"]["beatmap_id"]}"><b>${element["beatmap"]["song_name"]}</b></a><dt></div>`;
            new_scores += '<div class="score-mods">';
            if (mods != null) { for (i = 0; i <= mods.length - 1; i++) { new_scores += `<div tooltip="${mods_names[i]}"><img src="../resources/profile/mods/${mods[i].toString()}.png" height="25px"/></div>`; } }
            new_scores += `</div><div class="score-pp"><b>${element["accuracy"].toFixed(2)}%</b></div><div class="score-pp"><b>${element["pp"].toFixed()}pp</b><dt>`;
            new_scores += `<text class="score-acc">${(element["pp"] * 0.95 ** scores).toFixed()}pp</b></div></div>`;
            scores++;
        });
        $("#scores-data").html(new_scores);
        curr_mode = modID;
    });
}
