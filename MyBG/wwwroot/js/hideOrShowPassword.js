let fields = document.getElementsByClassName("password-input");
let buttons = document.getElementsByClassName("password-eye");

function hideOrShowPassword(index) {
    if(fields[index].type === "text") {
        fields[index].type = "password";
        buttons[index].src = "/textures/view.png";
    }
    else {
        fields[index].type = "text";
        buttons[index].src = "/textures/hide.png";
    }
}