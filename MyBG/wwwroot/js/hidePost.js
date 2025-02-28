let commentarea = document.getElementsByClassName("comment-area")[0];
let uploadButton = document.getElementById("upload-button");
let replyButtons = document.getElementsByClassName("reply-post-button");
let replyAreasToHide = document.getElementsByClassName("reply-area");

function hideCommentPost() {
    if(commentarea.value.trim() === "") {
        uploadButton.style.display = "none";
    }
    else {
        uploadButton.style.display = "block";
    }
}

function hideReplyPost(index) {
    if(replyAreasToHide[index].value.trim() === "") {
        replyButtons[index].style.display = "none";
    }
    else {
        replyButtons[index].style.display = "inline-block";
    }
}

hideCommentPost();