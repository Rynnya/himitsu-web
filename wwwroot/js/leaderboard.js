var current_mode = 0; var current_relax = 0;

function loadLeaderboard(modID, relax) {
    $("tbody").empty();
    var board = "";
    api('leaderboard', { mode: modID, relax: relax, p: 1, l: 50 }, function (data) {
        data["users"].forEach(element => {
            board += `<tr class="text-center">`;
            board += `<td>#${element["chosen_mode"]["global_leaderboard_rank"]}</td>`;
            board += `<td><img src="../resources/flags/${element["country"]}.png" width="30px" height="20px"/></td>`;
            board += `<td class="text-left"><a href="/u/${element["id"]}" class="link-button-black">${element["username"]}</a></td>`;
            board += `<td>${element["chosen_mode"]["pp"]}</td>`;
            board += `<td>${element["chosen_mode"]["accuracy"].toFixed(2)}%</td>`;
            board += `<td>${Math.floor(element["chosen_mode"]["level"])}</td>`;
            board += `<td>${element["chosen_mode"]["playcount"]}</td></tr>`;
        });
        $("tbody").html(board);
        current_mode = modID;
        current_relax = relax;
    })
}