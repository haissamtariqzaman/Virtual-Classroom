
function dropDownClick(a) {
    hideDropDown();
    document.getElementById(a).style.display = "initial";
}

window.onclick = function (event) {
    if (!event.target.matches('.dropDownS')) {
        hideDropDown();
    }
    if (!event.target.matches('.navButton' && '.navimage')) {
        hidenavdrop();
    }
}

function hideDropDown() {
    var x = document.getElementsByClassName("dropdown-content");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
}

    function popup() {
        document.getElementById("joinClassM").style.display = "flex";
    }

    function popup1() {
        document.getElementById("addClassM").style.display = "flex";
    }

    function popUpOff() {
        document.getElementById("joinClassM").style.display = "none";
        document.getElementById("addClassM").style.display = "none";
    }

    function hidenavdrop() {
        var y = document.getElementById('navDrop');
        y.style.display = "none";
    }

    function logout(a) {
        var x = document.getElementById(a);
        x.style.display = "block";
    }