let comments = document.getElementsByClassName("comment-div-parent");

function expand() {
    let count = Number(document.getElementById("countInput").value);
    showComments(count);
    document.getElementById("countInput").value = Number(document.getElementById("countInput").value) + 5;
}

function showComments(count) {
    for(let i = 0; i < count; i++) {
        if(i >= comments.length - 1 || comments.length === 0) {
            document.getElementById("showMore").style.display = "none";
            comments[i].style.display = "block";
            break;
        }
        comments[i].style.display = "block";
    }
}

expand()