let index2 = 0;

function addTransportWay() {
    let warning = document.getElementById("transportway-warning");
    let travelDiv = document.getElementById("transportWaysDiv");
    let fromInput = document.getElementById("from");
    let timeInput = document.getElementById("time");
    let minInput = document.getElementById("timeMins");
    let typeDiv = document.getElementById("transportWay");
    let imageTag;
    warning.style.display = "none";
    switch(Number(typeDiv.value))
    {
        case 0:
            imageTag = "<img src=\"/Textures/car.png\" class=\"page-way-icon\" />";
            break;
        case 1:
            imageTag = "<img src=\"/Textures/train.png\" class=\"page-way-icon\" />";
            break;
        case 2:
            imageTag = "<img src=\"/Textures/bus.png\" class=\"page-way-icon\" />";
            break;
        case 3:
            imageTag = "<img src=\"/Textures/plane.png\" class=\"page-way-icon\" />";
            break;
    }
    travelDiv.innerHTML = travelDiv.innerHTML.concat(`
    <div class="wayDiv"> 
        <input name="TransportWays[${index2}].TransportWayType" value="${typeDiv.value}"/>
        <input name="TransportWays[${index2}].TransportOrigin" value="${fromInput.value}"/>
        <input name="TransportWays[${index2}].TransportTimeHours" value="${timeInput.value}"/>
        <input name="TransportWays[${index2}].TransportTimeMinutes" value="${minInput.value}"/>
    </div>
    <p class="TransportWayDisplay">${imageTag} From ${fromInput.value}: ${timeInput.value}:${minInput.value}</p>
`);
    transportValid.value = ".";
    index2++;
}