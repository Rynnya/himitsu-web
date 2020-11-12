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
  document.getElementsByName('style')[0].attributes.value.textContent = device;
}
/*
osuMode = document.getElementById('osuSettingMode').value;
osuGameMode = document.getElementById('osuSettingGameMode').value;
console.log(osuMode, osuGameMode, device);*/
