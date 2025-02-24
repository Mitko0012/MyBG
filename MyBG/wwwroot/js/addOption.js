let index2 = 0;

function addTransportWay() {
    let travelDiv = document.getElementById("transportWaysDiv");
    let fromInput = document.getElementById("from");
    let timeInput = document.getElementById("time");
    let minInput = document.getElementById("timeMins");
    let typeDiv = document.getElementById("transportWay");
    travelDiv.innerHTML = travelDiv.innerHTML.concat(`
                                                    <div class="wayDiv"> 
                                                        <input name="TransportWays[${index2}].TransportWayType" value="${typeDiv.value}"/>
                                                        <input name="TransportWays[${index2}].TransportOrigin" value="${fromInput.value}"/>
                                                        <input name="TransportWays[${index2}].TransportTimeHours" value="${timeInput.value}"/>
                                                        <input name="TransportWays[${index2}].TransportTimeMinutes" value="${minInput.value}"/>
                                                    </div>
                                                    <div class="displayWayDiv">
                                                        <p>${typeDiv.Value}</p>
                                                        <p>${fromInput.value}</p>
                                                        <p>${timeInput.value}</p>
                                                        <p>${minInput.Value}</p>
                                                    </div>
`);
    transportValid.value = ".";
    index2++;
}