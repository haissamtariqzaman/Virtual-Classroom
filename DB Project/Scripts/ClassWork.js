window.onclick = function (event) {
    if (!event.target.matches('.navButton' && '.navimage')) {
        hidenavdrop();
    }
    if (!event.target.matches('.dropDownS')) {
        hideDropDown();
    }
}

function hidenavdrop() {
    var y = document.getElementById('navDrop');
    y.style.display = "none";
}

function logout(a) {
    var x = document.getElementById(a);
    x.style.display = "block";
}

function button1click() {
    var b1 = document.getElementById("sbutton1");
    var b2 = document.getElementById("sbutton2");
    b1.style.backgroundColor = "#1F2833";
    b1.style.color = "white";
    b2.style.backgroundColor = "white";
    b2.style.color = "#1F2833";

    var assign = document.getElementsByClassName("content");
    var lec = document.getElementsByClassName("lec");

    for (i = 0; i < lec.length; i++) {
        lec[i].style.display = "none";
    }

    for (i = 0; i < assign.length; i++) {
        assign[i].style.display = "initial";
    }

    var b3 = document.getElementById("button")
    b3.setAttribute('onclick',"openAdd('AddAssignmentM')");
}

function button2click() {
    var b2 = document.getElementById("sbutton1");
    var b1 = document.getElementById("sbutton2");
    b1.style.backgroundColor = "#1F2833";
    b1.style.color = "white";
    b2.style.backgroundColor = "white";
    b2.style.color = "#1F2833";

    var assign = document.getElementsByClassName("content");
    var lec = document.getElementsByClassName("lec");

    for (i = 0; i < assign.length; i++) {
        assign[i].style.display = "none";
    }

    for (i = 0; i < lec.length; i++) {
        lec[i].style.display = "initial";
    }

    var b3 = document.getElementById("button")
    b3.setAttribute('onclick', "openAdd('AddLectureM')");
}

function moreButton(id) {
    //var x = document.getElementsByClassName("downB");

    //for (i = 0; i < x.length; i++) {
    //    if (x[i] != id) {
    //        x[i].previousElementSibling.style.display = "none";
    //        x[i].style.display = "block";
    //    }
    //}

    id.style.display = "none";
    id.previousElementSibling.style.display = "block";
}

function uploadChanged(u,v) {
    var files = document.getElementById(u).files;

    var OldDiv = document.getElementsByClassName("uploadData");

    while (OldDiv[0]) {
        OldDiv[0].parentNode.removeChild(OldDiv[0]);
    }

    for (var i = 0; i < files.length; i++) {
        var div = document.createElement("div");
        div.className = "uploadData";

        var para = document.createElement("p");
        para.className = "uploadpara";
        para.innerHTML = files[i].name;

        var icon = document.createElement("i");
        icon.className = "fas fa-file-alt";
        icon.style.fontSize = "30px";

        div.appendChild(icon);
        div.appendChild(para);

        document.getElementById(v).appendChild(div);
    }
}

function openAdd(u) {
    var a = document.getElementById(u);
    a.style.display = "flex";
}

function closefunc(u) {
    var a = document.getElementById(u);
    a.style.display = "none";
}

function dropDownClick(a) {
    hideDropDown();
    document.getElementById(a).style.display = "initial";
}

function hideDropDown() {
    var x = document.getElementsByClassName("dropdown-content");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
}