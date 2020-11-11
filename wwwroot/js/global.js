let headerProfileVisibility = false;
function headerLogoButton() {

    var logo = document.getElementById('header-logo').style;
    var headerButtonText = document.getElementsByClassName("header-button-text");
    var container = document.getElementById("header-profile-container").style;
    var content = document.getElementById("content").style;

    if (headerProfileVisibility == false) {
        container.width = "150px";
        content.filter = "blur(2px)";
        logo.transform = "rotate(240deg)";
        for (let i of headerButtonText) {
            i.style.visibility = "visible";
            i.style.opacity = "1";
            i.style.transition = "0.2s 0.4s"
        }
        headerProfileVisibility = true;
    }

    else {
        container.width = "0px";
        content.filter = "blur(0px)";
        logo.transform = "rotate(0deg)";
        for (let i of headerButtonText) {
            i.style.visibility = "hidden";
            i.style.opacity = "0";
            i.style.transition = "0.2s"
        }
        headerProfileVisibility = false;
    }
}

function osuMode(mode) {
    let allOsuMode = [document.getElementById("mode-vanilla"), document.getElementById("mode-relax")];

    for (let i of allOsuMode) {
        i.style.color = "#888888";
        i.style.borderBottom = "none";
    }

    if (mode == 1) {
        document.getElementById('mode-mania').style.display = 'none';
    }
    else {
        document.getElementById('mode-mania').style.display = 'inline-block';
    }

    if (mode >= 0 && mode <= 1) {
        allOsuMode[mode].style.borderBottom = "solid 3px black";
        allOsuMode[mode].style.color = "black";
    }
}

function osuGameMode(gamemode) {
    let allOsuMode = [document.getElementById("mode-std"), document.getElementById("mode-taiko"), document.getElementById("mode-ctb"), document.getElementById("mode-mania")];

    for (let i of allOsuMode) {
        i.style.color = "#888888";
        i.style.borderBottom = "none";
    }

    if (gamemode == 3) {
        document.getElementById('mode-relax').style.display = 'none';
    }
    else {
        document.getElementById('mode-relax').style.display = 'inline-block';
    }

    if (gamemode >= 0 && gamemode <= 3) {
        allOsuMode[gamemode].style.borderBottom = "solid 3px black";
        allOsuMode[gamemode].style.color = "black";
    }
}

function _api(base, endpoint, data, success, failure, post, handleAllFailures) {
    if (typeof data == "function") {
        success = data;
        data = null;
    }
    if (typeof failure == "boolean") {
        post = failure;
        failure = undefined;
    }
    handleAllFailures = (typeof handleAllFailures !== undefined) ? handleAllFailures : false;

    $.ajax({
        method: (post ? "POST" : "GET"),
        dataType: "json",
        url: base + endpoint,
        data: (post ? JSON.stringify(data) : data),
        contentType: (post ? "application/json; charset=utf-8" : ""),
        success: function (data) {
            if (data.code != 200) {
                if (typeof failure === "function" &&
                    (handleAllFailures || (data.code >= 400 && data.code < 500))
                ) {
                    failure(data);
                    return;
                }
                console.warn(data);
            }
            success(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (typeof failure == "function" &&
                (handleAllFailures || (jqXHR.status >= 400 && jqXHR.status < 500))
            ) {
                failure(jqXHR.responseJSON);
                return;
            }
            console.warn(jqXHR, textStatus, errorThrown);
        },
    });
}

function api(endpoint, data, success, failure, post, handleAllFailures) {
    return _api("https://api.himitsu.ml/api/v1/", endpoint, data, success, failure, post, handleAllFailures);
}

function apiBancho(endpoint, data, success, failure, post, handleAllFailures) {
    return _api("https://c.himitsu.ml/api/v1/", endpoint, data, success, failure, post, handleAllFailures);
}

function timeFormat(t) {
    var h = Math.floor(t / 3600);
    t %= 3600;
    var m = Math.floor(t / 60);
    var s = t % 60;
    var c = "";
    if (h > 0) {
        c += h + ":";
        if (m < 10) {
            c += "0";
        }
        c += m + ":";
    } else {
        c += m + ":";
    }
    if (s < 10) {
        c += "0";
    }
    c += s;
    return c;
}

// thank mr stackoverflow
function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' +
            ',' +
            '$2');
    }
    return x1 + x2;
}

var modbits = {
    nomod: 0,
    nf: 1 << 0,
    ez: 1 << 1,
    td: 1 << 2,
    hd: 1 << 3,
    hr: 1 << 4,
    dt: 1 << 6,
    ht: 1 << 8,
    nc: 1 << 9,
    fl: 1 << 10,
    so: 1 << 12,
};
var modnames = {
    'NoFail': 'NF',
    'Easy': 'EZ',
    'Touch&nbsp;Device': 'TD',
    'Hidden': 'HD',
    'Hardrock': 'HR',
    'Double&nbsp;Time': 'DT',
    'Half&nbsp;Time': 'HT',
    'Nightcore': 'NC',
    'Flashlight': 'FL',
    'Spinout': 'SO'
}
modnames.toString = function (mods) {
    var res = "";
    if (mods == null) return '/';
    for (var property in modnames) {
        mods.forEach(mod => {
            if (mod == modnames[property]) {
                res += property + '/';
            }
        })
    }
    if (res.indexOf("Double Time") >= 0 && res.indexOf("Nightcore") >= 0) {
        res = res.replace("Double Time", "");
    }
    res = res.substring(0, res.length - 1);
    return res;
}
modbits.toString = function (mods) {
    var res = "";
    for (var property in modbits) {
        if (property.length != 2) {
            continue;
        }
        if (!modbits.hasOwnProperty(property)) {
            continue;
        }
        if (mods & modbits[property]) {
            res += property.toUpperCase();
        }
    }
    if (res.indexOf("DT") >= 0 && res.indexOf("NC") >= 0) {
        res = res.replace("DT", "");
    }
    return res;
}

function displayContent(){
  document.getElementById('content').style.display = "flex";
}

function selectors(){
var x, i, j, l, ll, selElmnt, a, b, c;
/*look for any elements with the class "custom-select":*/
x = document.getElementsByClassName("custom-select");
l = x.length;
for (i = 0; i < l; i++) {
  selElmnt = x[i].getElementsByTagName("select")[0];
  ll = selElmnt.length;
  /*for each element, create a new DIV that will act as the selected item:*/
  a = document.createElement("DIV");
  a.setAttribute("class", "select-selected");
  a.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
  x[i].appendChild(a);
  /*for each element, create a new DIV that will contain the option list:*/
  b = document.createElement("DIV");
  b.setAttribute("class", "select-items select-hide");
  for (j = 1; j < ll; j++) {
    /*for each option in the original select element,
    create a new DIV that will act as an option item:*/
    c = document.createElement("DIV");
    c.innerHTML = selElmnt.options[j].innerHTML;
    c.addEventListener("click", function(e) {
      /*when an item is clicked, update the original select box,
      and the selected item:*/
      var y, i, k, s, h, sl, yl;
      s = this.parentNode.parentNode.getElementsByTagName("select")[0];
      sl = s.length;
      h = this.parentNode.previousSibling;
      for (i = 0; i < sl; i++) {
        if (s.options[i].innerHTML == this.innerHTML) {
          s.selectedIndex = i;
          h.innerHTML = this.innerHTML;
          y = this.parentNode.getElementsByClassName("same-as-selected");
          yl = y.length;
          for (k = 0; k < yl; k++) {
            y[k].removeAttribute("class");
          }
          this.setAttribute("class", "same-as-selected");
          break;
        }
      }
      h.click();
    });
    b.appendChild(c);
  }
  x[i].appendChild(b);

  a.addEventListener("click", function(e) {
    /*when the select box is clicked, close any other select boxes,
    and open/close the current select box:*/
    e.stopPropagation();
    closeAllSelect(this);
    this.nextSibling.classList.toggle("select-hide");
    this.classList.toggle("select-arrow-active");
  });
}}

function closeAllSelect(elmnt) {
  /*a function that will close all select boxes in the document,
  except the current select box:*/
  var x, y, i, xl, yl, arrNo = [];
  x = document.getElementsByClassName("select-items");
  y = document.getElementsByClassName("select-selected");
  xl = x.length;
  yl = y.length;
  for (i = 0; i < yl; i++) {
    if (elmnt == y[i]) {
      arrNo.push(i)
    } else {
      y[i].classList.remove("select-arrow-active");
    }
  }
  for (i = 0; i < xl; i++) {
    if (arrNo.indexOf(i)) {
      x[i].classList.add("select-hide");
    }
  }
}
/*if the user clicks anywhere outside the select box,
then close all select boxes:*/
document.addEventListener("click", closeAllSelect);
