window.googleAuth = {
    init: function (clientId, dotNetHelper, successCallbackName) {
        google.accounts.id.initialize({
            client_id: clientId,
            callback: function (response) {
                dotNetHelper.invokeMethodAsync(successCallbackName, response.credential);
            }
        });
    }
};

window.triggerGoogleSignIn = function () {
    // Opens the Google sign-in one-tap prompt / popup
    google.accounts.id.prompt();
};