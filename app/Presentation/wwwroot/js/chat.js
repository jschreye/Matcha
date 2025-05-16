// Fonction pour faire défiler la fenêtre de chat vers le bas
window.scrollToBottom = function (element) {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}; 