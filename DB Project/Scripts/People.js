function dropDownClick(a) {
    hideDropDown();
    document.getElementById(a).style.display = "initial";
}

window.onclick = function (event) {
    if (!event.target.matches('.navButton' && '.navimage')) {
        hidenavdrop();
    }
    if (!event.target.matches('.closeB')) {
        hideDropDown();
    }

}

function hideDropDown() {
    var x = document.getElementsByClassName("dropdown-content");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
}

function popup() {
    document.getElementById("AddPeopleM").style.display = "flex";
}

function popUpOff() {
    document.getElementById("AddPeopleM").style.display = "none";
}

function hidenavdrop() {
    var y = document.getElementById('navDrop');
    y.style.display = "none";
}

function logout(a) {
    var x = document.getElementById(a);
    x.style.display = "block";
}