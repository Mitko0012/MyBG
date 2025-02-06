let index = 0;

function AddOption() {
    let travelDiv = document.getElementById("TransportWaysDiv");
    let fromInput = document.getElementById("From");
    let timeInput = document.getElementById("Time");
    let typeDiv = document.getElementById("TransportWay");
    travelDiv.innerHTML = travelDiv.innerHTML.concat(`
                                                    <div id="WayDiv${index}"> 
                                                        <input name="TransportWays[${index}].TransportWayType" value="${typeDiv.value}"/>
                                                        <input name="TransportWays[${index}].TransportOrigin" value="${fromInput.value}">
                                                        <input name="TransportWays[${index}].TransportTime" value="${timeInput.value}">
                                                    </div>`);
    index++;
}