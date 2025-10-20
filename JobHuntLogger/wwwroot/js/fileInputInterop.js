//clears the html element, for when the StateHasChanged doesn't update the values
window.fileInputInterop = {
    clearFileInput: function (id) {
        const el = document.getElementById(id);
        if (el) {
            try {
                el.value = '';
            } catch (e) {
                console && console.warn && console.warn('clearFileInput failed', e);
            }
        }
    }
};