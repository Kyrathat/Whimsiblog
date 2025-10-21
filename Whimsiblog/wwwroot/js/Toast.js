// Dismisses all .toast-success elements
(function () {
    window.hideSuccessToastsAfter = function (ms) { // Global function
        setTimeout(() => {
            document.querySelectorAll('.toast-success').forEach(t => t.remove()); // Selects all elements with "toast-success", then 
        }, ms ?? 3000);                                                                             // loops through those elements and removes them from the DOM
    };
})();
