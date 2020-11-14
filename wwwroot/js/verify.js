function checkVerify(user) {
    setInterval(function () {
        $.getJSON("https://c.himitsu.ml/api/v1/verifiedStatus?u=" + user,
            function (data) {
                console.log(data.result);
                if (data.result >= 0) {
                    window.location.href = "/";
                }
            })
    }, 5000)
}