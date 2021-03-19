function uploadChanged() {
    var files = document.getElementById("upload").files;

    var OldDiv = document.getElementsByClassName("uploadData");

    while(OldDiv[0]) {
        OldDiv[0].parentNode.removeChild(OldDiv[0]);
    }

    for (var i = 0; i < files.length; i++) {
        var div = document.createElement("div");
        div.className = "uploadData";

        var para = document.createElement("p");
        para.innerHTML = files[i].name;

        var icon = document.createElement("i");
        icon.className = "fas fa-file-alt";
        icon.style.fontSize = "30px";

        div.appendChild(icon);
        div.appendChild(para);

        document.getElementById("uploads").appendChild(div);
    }

}