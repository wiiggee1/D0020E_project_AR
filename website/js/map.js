document.addEventListener('DOMContentLoaded', () => {
    
    // Used coordinates
    const luleCoords = [65.58438073922254, 22.159403177945457];
    const ltuCoords = [65.61818932415491, 22.140257153674423];
    const model1Coords = [65.61884604121137, 22.142276537103953];

    // Models and other external layers    
    const model1 = ["https://mapwarper.net/maps/tile/70518/{z}/{x}/{y}.png", "Game model 1"];
    const fHouse = ["https://mapwarper.net/maps/tile/70595/{z}/{x}/{y}.png", "F house"];

    // Rendering map and different background layers
    var map2d = L.map('map2d').setView(luleCoords, 12);
    var osm = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map2d);

    var bike = L.tileLayer('https://{s}.tile-cyclosm.openstreetmap.fr/cyclosm/{z}/{x}/{y}.png', {
        attribution: '<a href="https://github.com/cyclosm/cyclosm-cartocss-style/releases" title="CyclOSM - Open Bicycle render">CyclOSM</a> | Map data: &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    });

    var esri = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
        attribution: 'Tiles &copy; Esri &mdash; Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community'
    });
    var darkMode = L.tileLayer('https://tiles.stadiamaps.com/tiles/alidade_smooth_dark/{z}/{x}/{y}{r}.png', {
        attribution: '&copy; <a href="https://stadiamaps.com/">Stadia Maps</a>, &copy; <a href="https://openmaptiles.org/">OpenMapTiles</a> &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors'
    });

    var backgrounds = {
        "Normal": osm,
        "Dark mode": darkMode,
        "Bike": bike,
        "Satelite": esri
    };

    // LTU logo
    var ltuLogo = L.icon({ iconUrl: '/img/LTU_L_sve_bla.png', iconSize: [50, 50] });
    var ltu = L.marker(ltuCoords, {icon: ltuLogo}).addTo(map2d);

    // Layers to be turned on/off
    var checkboxes = { "LTU": ltu };
    var layerControl = L.control.layers(backgrounds, checkboxes).addTo(map2d);

    // Constants from other files
    const startBtn = document.getElementById('startbtn');
    const ltuMarker = document.querySelector('img[alt="Marker"]');
    
    // TODO
    function renderOnMap(object){
        var obj = L.tileLayer(object[0], {}).addTo(map2d);         
        layerControl.addOverlay(obj, object[1]);
    }

    // Upon pressing start
    if (startBtn) {
        startBtn.addEventListener('click', () => {
            map2d.setView(model1Coords, 20);
            renderOnMap(model1);
        });
    }

    // Upon clicking LTU's logo
    if (ltuMarker) {
        ltuMarker.addEventListener('click', () => {
            map2d.setView(ltuCoords, 16);
            renderOnMap(fHouse);
        });        
    }
});