function fillMentions() {
    let comments = document.getElementsByClassName("comment-text-data");
    for(let i = 0; i < comments.length; i++) {
        let arr = comments[i].innerHTML.split(" ");
        let updatedString = "";
        for(let j = 0; j < arr.length; j++) {
            let str = arr[j];
            if(str[0] === '@') {
                str = `<a href="/User/UserPage?userName=${str.slice(1)}">${str}</a>`;
            }
            updatedString += str + " ";
        }
        comments[i].innerHTML = updatedString.trim(); 
    }
}