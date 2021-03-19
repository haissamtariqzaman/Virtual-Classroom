function checkIfImage() {
    var file = document.getElementById("pic").files[0];
    if (file['type'].split('/')[0] === 'image') {
        return true;
    }
    alert("Select an image!");
    return false;
}