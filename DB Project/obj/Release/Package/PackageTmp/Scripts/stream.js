
function uploadChanged() {
    uploadButton = document.getElementById("upload1");
    uploadText = document.getElementById("uploadText");
    if (uploadButton.files.length == 0) {
        uploadText.innerHTML = "";
    }
    else if (uploadButton.files.length == 1) {
        uploadText.innerHTML = uploadButton.files.length + " file selected";
    }
    else {
        uploadText.innerHTML = (uploadButton.files.length + " files selected");
    }

}