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
    return response.ok;
}

window.refreshProfileClaim = async function() {
    const response = await fetch('/auth/refresh-profile-claim', {
        method: 'POST',
        credentials: 'include' // essentiel pour inclure et mettre à jour le cookie
    });
    return response.ok;
}