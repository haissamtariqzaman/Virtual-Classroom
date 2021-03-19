function inputValidation() {
    var user = document.getElementById("eml").value;
    var password = document.getElementById("pass1").value;
    var date1 = document.getElementById("ddobb").value;

    var found = 0;

    for (x = 0; x < user.length; x++) {
        if (user[x] == '@') {
            found = 1;
            break;
        }
    }

    if (found == 0) {
        alert("This email already exists!");
        return false;
    }

    if (password.length < 8) {
        alert("Password should be of atleast 8 characters!");
        return false;
    }

    var bday = +new Date(date1);

    if ((Date.now() - bday) / 31557600000 < 10) {
        alert("Invalid date!");
        return false;
    }

    return true;
}