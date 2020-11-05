function initTimer(user) {
    setInterval(checkVerify(user), 5000);
}

function checkVerify(user) {
    apiBancho("verifiedStatus?u=" + user, function(data) {
        if (data.result >= 0) {
            window.location.href = "/";
        }
    });
}