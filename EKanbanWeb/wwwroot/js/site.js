function addZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}

function GetFormattedDateTime(dt) {
    formattedDate = GetFormattedDate(dt);
    formattedTime = GetFormattedTime(dt);
    return formattedDate + " " + formattedTime;
}

function GetFormattedDate(dt) {
    if (dt == null) return "";
    var dt = new Date(dt);
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    //var dd = dt.getDate() < 10 ? '0' + dt.getDate() : dt.getDate();
    var dd = addZero(dt.getDate());
    var mmm = months[dt.getMonth()];
    var yyyy = dt.getFullYear();
    var formattedDate = dd + "-" + mmm + "-" + yyyy;
    return formattedDate;
}

function GetFormattedTime(dt) {
    if (dt == null) return "";
    var dt = new Date(dt);
    var h = addZero(dt.getHours());
    var m = addZero(dt.getMinutes());
    var formattedTime = h + ":" + m;
    return formattedTime;
}
