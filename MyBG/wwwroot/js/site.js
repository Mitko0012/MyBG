let index = 0;
const searchSelect = document.getElementById("searchSelect");

function addOption() {
    let travelDiv = document.getElementById("transportWaysDiv");
    let fromInput = document.getElementById("from");
    let timeInput = document.getElementById("time");
    let typeDiv = document.getElementById("transportWay");
    travelDiv.innerHTML = travelDiv.innerHTML.concat(`
                                                    <div id="wayDiv${index}"> 
                                                        <input name="TransportWays[${index}].TransportWayType" value="${typeDiv.value}"/>
                                                        <input name="TransportWays[${index}].TransportOrigin" value="${fromInput.value}"/>
                                                        <input name="TransportWays[${index}].TransportTime" value="${timeInput.value}"/>
                                                    </div>`);
    index++;
}

function showOrHideSearch() {
    let search = document.getElementById("search");
    if(searchSelect.value === "Search") {
        search.style.display = "inline";
    }
    else {
        search.style.display = "none";
    }
}