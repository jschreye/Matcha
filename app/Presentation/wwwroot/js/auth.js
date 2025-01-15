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

async function logout() {
    const response = await fetch('/auth/logout', {
        method: 'POST',
        credentials: 'include'
    });
    return response.ok;
}