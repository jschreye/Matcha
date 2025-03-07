/** Récupère la géolocalisation via le navigateur */
window.getUserLocation = () => {
    return new Promise((resolve, reject) => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    resolve({ 
                        Latitude: position.coords.latitude, 
                        Longitude: position.coords.longitude 
                    });
                },
                (error) => reject(error)
            );
        } else {
            reject("Géolocalisation non supportée");
        }
    });
};

/** Récupère une localisation alternative (exemple via l'IP) */
window.getFallbackLocation = () => {
    // Pour l'exemple, on renvoie des coordonnées par défaut
    return Promise.resolve({ Latitude: 46.53256846074302, Longitude: 6.591996021443265 });
};

/** Récupère le statut de localisation de l'utilisateur depuis l'API */
window.getUserLocalisationStatus = async () => {
    try {
        const response = await fetch('/auth/user-info', { 
            method: 'GET', 
            credentials: 'include' 
        });
        if (!response.ok) return JSON.stringify({ localisationIsActive: false });
        const data = await response.json();
        return JSON.stringify(data);
    } catch (error) {
        console.error("Erreur lors de la récupération du statut de localisation:", error);
        return JSON.stringify({ localisationIsActive: false });
    }
};