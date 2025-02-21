let showHideReplyButtons = document.getElementsByClassName("showHideReplies");
let repliesDivs = document.getElementsByClassName("repliesDiv");
let showAddReplyPs = document.getElementsByClassName("addReplyButt");
let hideAddReplyButts = document.getElementsByClassName("cancelReply");
let addReplyForms = document.getElementsByClassName("replyForm");

function showHideReplies(index) {
    let button = showHideReplyButtons[index];
    let divToHideOrShow = repliesDivs[index];
    if(divToHideOrShow.style.display === "block") {
        button.innerHTML = "Show replies";  
        divToHideOrShow.style.display = "none";
    }
    else {
        button.innerHTML = "Hide replies";  
        divToHideOrShow.style.display = "block";  
    }
}

function showAddReply(index) {
    addReplyForms[index].style.display = "block";
    showAddReplyPs[index].style.display = "none";
}

function hideAddReply(index) {
    addReplyForms[index].style.display = "none";
    showAddReplyPs[index].style.display = "block";
}