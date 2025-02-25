let fileInput = document.getElementById("fileCont");
let message = document.getElementById("input-msg");

fileInput.addEventListener("change", updateMsg);

function updateMsg() {
    let fileName = fileInput.files.length > 0 ? fileInput.files[0].name : 'No file chosen';
    message.textContent = fileName;
}