let index = 0;
let map;
let marker;
let xInput = document.getElementById("xInput");
let yInput = document.getElementById("yInput");
let latInfo = document.getElementById("latInfo");
let longInfo = document.getElementById("longInfo");
let transportValid = document.getElementById("verify-transport");
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
    transportValid.value = ".";
    index++;
}

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
    if (searchSelect.value === "Destination") {
        destination.style.display = "inline";
    }
    else {
        destination.style.display = "none";
    }
}

function initMap() {
    map = L.map("map").setView([42.766109, 25.238558], 8);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    marker = L.marker([42.766109, 25.238558], {draggable: true}).addTo(map);
    marker.on("move", () => {
        xInput.value = marker.getLatLng().lat;
        yInput.value = marker.getLatLng().lng;
    }
    );
}

function reverseUpdatePos(event) {
    marker.setLatLng([xInput.value, yInput.value])
}

function initMapDisplay() {
    map = L.map("map").setView([latInfo.value, longInfo.value], 9);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    marker = L.marker([latInfo.value, longInfo.value]).addTo(map);
}
