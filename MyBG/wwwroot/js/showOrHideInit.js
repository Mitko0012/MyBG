showOrHideSearch();

function showOrHideSearch() {
    let search = document.getElementById("search");
    let region = document.getElementById("regionSelect");
    let destination = document.getElementById("typeSelect");
    if(searchSelect.value === "Search") {
        search.style.display = "inline";
    }
    else {
        search.style.display = "none";
    }
    if (searchSelect.value === "Region") {
        region.style.display = "inline";
    }
    else {
        region.style.display = "none";
    }
    if (searchSelect.value === "Destination" || searchSelect.value === "CultureType") {
        destination.style.display = "inline";
    }
    else {
        destination.style.display = "none";
    }
}