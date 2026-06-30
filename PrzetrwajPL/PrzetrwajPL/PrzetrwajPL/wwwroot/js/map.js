// wwwroot/js/map.js

window.initializePolandMap = (containerId, markersData) => {
	const mapElement = document.getElementById(containerId);
	if (!mapElement) return;

	// center the map on Poland (52.0, 19.0)
	// Zoom = 7
	const map = L.map(containerId).setView([52.0689, 19.4797], 7);

	// visual layer from OpenStreetMap
	L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
		attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
	}).addTo(map);

	// map and set points
	if (markersData && Array.isArray(markersData)) {
		markersData.forEach(marker => {
			if (marker.lat && marker.long) {

				// make marker and add a popup with title
				L.marker([marker.lat, marker.long])
					.addTo(map)
					.bindPopup(`
						<div style="font-family: sans-serif;">
							<h4 style="margin: 0 0 5px 0; color: #333;">${marker.title}</h4>
							<p style="margin: 0; font-size: 12px; color: #666;">Kategoria: ${marker.idCategory}</p>
							<a href="/post/${marker.idPost}" style="display:inline-block; margin-top:8px; font-size:12px; color:#0094ff; text-decoration:none; font-weight:bold;">Zobacz szczegóły &raquo;</a>
						</div>
					`);
			}
		});
	}
};


// auto get map position of the user
window.getUserLocation = (dotNetHelper) => {
	if (navigator.geolocation) {
		navigator.geolocation.getCurrentPosition(
			(position) => {
				dotNetHelper.invokeMethodAsync('SetLocation', position.coords.latitude, position.coords.longitude);
			},
			(error) => {
				dotNetHelper.invokeMethodAsync('LocationError', "Nie udało się pobrać lokalizacji: " + error.message);
			}
		);
	} else {
		dotNetHelper.invokeMethodAsync('LocationError', "Geolokalizacja nie jest wspierana przez Twoją przeglądarkę.");
	}
};


// global registry for storing maps
window.activeMaps = window.activeMaps || {};

// manual picking mode (with click)
window.initializeLocationPickerMap = (containerId, dotNetHelper) => {
	const mapElement = document.getElementById(containerId);
	if (!mapElement) return;

	//if map already exists => exit
	if (window.activeMaps[containerId]) {
		setTimeout(() => { window.activeMaps[containerId].map.invalidateSize(); }, 50);
		return;
	}

	// center the map on Poland (52.0, 19.0) | Zoom = 7
	const map = L.map(containerId).setView([52.0689, 19.4797], 7);

	L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
		attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
	}).addTo(map);

	// FIX: forces leaflet to recalculate the DIV size (again)
	setTimeout(() => {
		map.invalidateSize();
	}, 50);

	let currentMarker = null;

	// save the map in global registry
	window.activeMaps[containerId] = {
		map: map,
		getMarker: () => currentMarker,
		setMarker: (marker) => { currentMarker = marker; }
	};

	map.on('click', function (e) {
		const lat = e.latlng.lat;
		const lng = e.latlng.lng;

		//we let the Blazor decide whether or not to place the marker on the map
		//window.setLocationOnMap(containerId, lat, lng);

		// report back the location to Blazor
		dotNetHelper.invokeMethodAsync('SetLocation', lat, lng);
	});
};

// sets a marker on the map
window.setLocationOnMap = (containerId, lat, lng) => {
	const mapData = window.activeMaps[containerId];
	if (!mapData) return;

	const map = mapData.map;
	let currentMarker = mapData.getMarker();
	const latlng = [lat, lng];

	if (currentMarker) {
		currentMarker.setLatLng(latlng);
	} else {
		currentMarker = L.marker(latlng).addTo(map);
		mapData.setMarker(currentMarker);
	}

	// center the map on this point
	map.setView(latlng);
};
