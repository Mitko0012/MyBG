let commentDisplayInputs = document.getElementsByClassName("comment-display-input");
let commentDisplayMainInput = document.getElementById("countInput");
let commentPostInput = document.getElementById("comment-post-display-input");
let replyPostInputs = document.getElementsByClassName("reply-post-display-input");
let commentScrollInput = document.getElementById("comment-post-scroll-input");
let scrollInput = document.getElementById("scrollInput");
let commentScrollInputs = document.getElementsByClassName("comment-scroll-input");
let replyScrollInputs = document.getElementsByClassName("reply-scroll-input");
let replies = document.getElementsByClassName("repliesDiv");
let replyString = document.getElementById("comment-post-string-input");
let replyReplyStrings = document.getElementsByClassName("reply-post-string-input");
let replyLikeInputs = document.getElementsByClassName("comment-reply-input")
let replyInput = document.getElementById("replyInput");
let showHideReplyButtonsToChange = document.getElementsByClassName("showHideReplies");

function onLike(index) {
    commentDisplayInputs[index].value = Number(commentDisplayMainInput.value) - 5;
    commentScrollInputs[index].value = window.scrollY;
    replyLikeInputs[index].value = getDisplayStrings();
}

function onPostComment() {
    commentPostInput.value = Number(commentDisplayMainInput.value) - 5;
    commentScrollInput.value = window.scrollY;
    replyString.value = getDisplayStrings();
}

function onPostReply(index) {
    replyPostInputs[index].value = Number(commentDisplayMainInput.value) - 5;
    replyScrollInputs[index].value = window.scrollY;
    replyReplyStrings[index].value = getDisplayStrings();
}

function onLikeForum(index) {
    commentScrollInputs[index].value = window.scrollY;
    replyLikeInputs[index].value = getDisplayStrings();
}

function onPostCommentForum() {
    commentScrollInput.value = window.scrollY;
    replyString.value = getDisplayStrings();
}

function onPostReplyForum(index) {
    replyScrollInputs[index].value = window.scrollY;
    replyReplyStrings[index].value = getDisplayStrings();
}

function getDisplayStrings() {
    let string = "";
    let index = 0;
    for(let i = 0; i < replies.length; i++) {
        if(replies[i].style.display == "block") {
            string += `${i} `;
        }
    }
    return string.trim();
}

for(let i of replyInput.value.split(" ")) {
    let num = Number(i);
    if(!Number.isNaN(num)) {
        replies[num].style.display = "block";
        showHideReplyButtonsToChange[num].textContent = "Hide replies";
    }
}
window.scrollTo(0, Number(document.getElementById("scrollInput").value));