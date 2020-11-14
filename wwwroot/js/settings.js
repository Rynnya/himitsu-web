function fill(osuMode, osuGameMode, device){
  let y = document.getElementById('osuSettingMode');
  yy = y.getElementsByTagName('option');

  let z = document.getElementById('osuSettingGameMode');
  zz = z.getElementsByTagName('option');

  if (osuMode != null){
    yy[osuMode + 1].selected = "selected";
  }
  else{
    yy[0].selected = "selected";
  }

  if (osuGameMode != null){
    zz[osuGameMode + 1].selected = "selected";
  }
  else{
    zz[0].selected = "selected";
  }

  let container = document.getElementById('device');
  selector = container.getElementsByTagName('input');

  if (device / 8 >= 1){
    selector[3].checked = true;
    device -= 8;
  }
  if (device / 4 >= 1){
    selector[2].checked = true;
    device -= 4;
  }
  if (device / 2 >= 1){
    selector[1].checked = true;
    device -= 2;
  }
  if (device / 1 >= 1){
    selector[0].checked = true;
    device -= 1;
  }
}

function selected(submode, std_o, relax_o, relax_o, taiko_p, ctb_p, mania_p) {
    document.getElementsByName("submode")[0].options[submode + 1].selected = "selected";
    document.getElementsByName("vanilla_order")[0].options[std_o + 1].selected = "selected";
    document.getElementsByName("relax_order")[0].options[relax_o + 1].selected = "selected";
    document.getElementsByName("std_prior")[0].options[relax_o + 1].selected = "selected";
    document.getElementsByName("taiko_prior")[0].options[taiko_p + 1].selected = "selected";
    document.getElementsByName("ctb_prior")[0].options[ctb_p + 1].selected = "selected";
    document.getElementsByName("mania_prior")[0].options[mania_p + 1].selected = "selected";
}

function selectedDevice(){
  device = 0;
  selected = 0;
  let container = document.getElementById('device');
  selector = container.getElementsByTagName('input');
  for (let i of selector){
    if (i.checked){
      device += parseInt(i.value);
      selected += 1;
    }
  }
  for (let i of selector){
    if (selected >= 2){
      i.style.display = "none";

      if (i.checked){
        i.style.display = "block";
      }
    }
    else{
      i.style.display = "block";
    }
  }
  document.getElementsByName("style")[0].defaultValue = device;
}

function previewAvatar() {
  $("#ava")
    .change(function (e) {
      var f = e.target.files;
      if (f.length < 1) {
        return;
      }
      var u = window.URL.createObjectURL(f[0]);
      var i = $("#avat")[0];
      i.src = u;
      i.onload = function () { window.URL.revokeObjectURL(this.src); };
    });
}

function resetAvatar(url){
  document.getElementById('avat').src = url;
}

function resetBg(url){
  document.getElementById('setting-bg').src = url;
}

function userpageBg(){
  let bg = document.getElementById('setting-bg-input').value;
  document.getElementById('setting-bg').src = bg;
}

function parseBB(){
    var lastCD = null;
    $("textarea[name='new_page']").on('input', function () {
        if (lastCD !== null) {
            clearTimeout(lastCD);
        }
        lastCD = setTimeout(function () {
            if ($("textarea[name='new_page']").val() !== null) {
                $.post("/settings/userpage/parse", { code: $("textarea[name='new_page']").val() }, function (data) {
                    $("#userpage-preview").html(data);
                }, "text");
            }
        }, 1000)
    });
}

function positionBg(){
  let bg = document.getElementsByClassName('demo')[0];
  let link = document.getElementsByName('back')[0].value
  let hor = document.getElementsByName('horiz')[0].value
  let ver = document.getElementsByName('vert')[0].value
  bg.style.background = hor + "%" + "" + ver + "%" + "" + "url(" + link + ")";
  bg.style.backgroundRepeat = "no-repeat";
  bg.style.backgroundSize = "cover";
}

function demo(vis){
  if (vis == 1){
  document.getElementsByClassName('demo')[0].style.visibility = "visible";
  document.getElementsByClassName('demo')[0].style.opacity = "1";
  }
  if (vis == 0){
  document.getElementsByClassName('demo')[0].style.visibility = "collapse";
  document.getElementsByClassName('demo')[0].style.opacity = "0";
  }
}
