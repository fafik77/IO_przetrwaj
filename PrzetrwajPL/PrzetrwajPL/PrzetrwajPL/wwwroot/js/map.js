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