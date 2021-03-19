window.onclick = function (event) {
    if (!event.target.matches('.navButton' && '.navimage')) {
        hidenavdrop();
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