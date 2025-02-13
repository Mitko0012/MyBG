let index = 0;
let map;
let marker;
let xInput = document.getElementById("xInput");
let yInput = document.getElementById("yInput");
const searchSelect = document.getElementById("searchSelect");

function addOption() {
    let travelDiv = document.getElementById("transportWaysDiv");
    let fromInput = document.getElementById("from");
    let timeInput = document.getElementById("time");
    let typeDiv = document.getElementById("transportWay");
    travelDiv.innerHTML = travelDiv.innerHTML.concat(`
                                                    <div class="wayDiv"> 
                                                        <input name="TransportWays[${index}].TransportWayType" value="${typeDiv.value}"/>
                                                        <input name="TransportWays[${index}].TransportOrigin" value="${fromInput.value}"/>
                                                        <input name="TransportWays[${index}].TransportTime" value="${timeInput.value}"/>
                                                    </div>`);
    index++;
}

function showOrHideSearch() {
    let search = document.getElementById("search");
    let region = document.getElementById("regionSelect");
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
}

function initMap() {
    map = L.map("map").setView([42.766109, 25.238558], 8);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    marker = L.marker([42.766109, 25.238558]).addTo(map);
    marker.options.interactive = true;
    marker.options.draggable = true;
}


function updatePos() {
    xInput.value = marker.getLatLng().lat;
    yInput.value = marker.getLatLng().lng; 
}

function reverseUpdatePos() {
    marker.setLatLng([xInput.value, yInput.value])
}
