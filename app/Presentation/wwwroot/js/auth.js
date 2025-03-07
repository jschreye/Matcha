async function login(username, password) {
    const response = await fetch('/auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        credentials: 'include',  // Inclut les cookies dans la requête
        body: new URLSearchParams({ 
            username: username, 
            password: password 
        })
    });
    
    if (response.ok) {
        // Après une connexion réussie, vérifier si on doit demander la géolocalisation
        const userInfo = await fetch('/auth/user-info', {
            method: 'GET',
            credentials: 'include'
        }).then(res => res.json());
        
        if (!userInfo.localisationIsActive) {
            // Demander à l'utilisateur s'il souhaite être géolocalisé
            const wantsGeolocation = confirm("Souhaitez-vous activer la géolocalisation pour améliorer votre expérience ?");
            
            // Mettre à jour la localisation en fonction de la réponse
            let locationData;
            if (wantsGeolocation) {
                try {
                    locationData = await window.getUserLocation();
                } catch (error) {
                    console.error("Erreur de géolocalisation:", error);
                    locationData = await window.getFallbackLocation();
                }
            } else {
                locationData = await window.getFallbackLocation();
            }
            
            // Envoyer les données de localisation au serveur
            await fetch('/auth/update-location', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({
                    latitude: locationData.Latitude,
                    longitude: locationData.Longitude,
                    localisationIsActive: wantsGeolocation  // Important: mettre à jour le statut en fonction de la réponse
                })
            });
        }
    }
    
    return response.ok;
}

window.refreshProfileClaim = async function() {
    const response = await fetch('/auth/refresh-profile-claim', {
        method: 'POST',
        credentials: 'include' // essentiel pour inclure et mettre à jour le cookie
    });
    return response.ok;
}