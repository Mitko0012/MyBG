
    let lat = Number(latInfo.value.replace(",", "."));
    let long = Number(longInfo.value.replace(",", "."));
    map = L.map("map").setView([lat, long], 9);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    marker = L.marker([lat, long]).addTo(map);
