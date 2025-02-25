let index2 = 0;

function addTransportWay() {
    let warning = document.getElementById("transportway-warning");
    let travelDiv = document.getElementById("transportWaysDiv");
    let fromInput = document.getElementById("from");
    let timeInput = document.getElementById("time");
    let minInput = document.getElementById("timeMins");
    let typeDiv = document.getElementById("transportWay");
    warning.style.display = "none";
    travelDiv.innerHTML = travelDiv.innerHTML.concat(`
                                                    <div class="wayDiv"> 
                                                        <input name="TransportWays[${index2}].TransportWayType" value="${typeDiv.value}"/>
                                                        <input name="TransportWays[${index2}].TransportOrigin" value="${fromInput.value}"/>
                                                        <input name="TransportWays[${index2}].TransportTimeHours" value="${timeInput.value}"/>
                                                        <input name="TransportWays[${index2}].TransportTimeMinutes" value="${minInput.value}"/>
                                                    </div>
                                                    <p class="TransportWayDisplay">${typeDiv.value} From ${fromInput.value}: ${timeInput.value}:${minInput.value}</p>
`);
    transportValid.value = ".";
    index2++;
}