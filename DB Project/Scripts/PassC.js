function validate() {
    var newp = document.getElementById("new").value;
    var confirm = document.getElementById("confirm").value;

    if (newp.length < 8) {
        alert("Password should be of atleast 8 characters!");
        return false;
    }

    if (newp.localeCompare(confirm) != 0) {
        alert("Password doesn't match!");
        return false;
    }

    return true;
}