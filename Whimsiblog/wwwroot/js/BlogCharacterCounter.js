(function () {
    function parseMax(textarea, fallback) {
        const attr = textarea.getAttribute('maxlength')
            ?? textarea.getAttribute('data-val-length-max'); // unobtrusive validation
        const n = parseInt(attr, 10);
        return Number.isFinite(n) ? n : fallback;
    }

    function wire(textarea, counter, fallbackMax = 1000) {
        const max = parseMax(textarea, fallbackMax);

        function update() {
            const len = (textarea.value ?? '').length;
            counter.textContent = `${len} / ${max} characters`;
            counter.classList.toggle('text-danger', len > max);
        }

        textarea.addEventListener('input', update);
        update(); // initial render
    }

    // Public API
    window.TextareaCounter = {
        initByIds: function (textareaId, counterId, fallbackMax) {
            const ta = document.getElementById(textareaId);
            const counter = document.getElementById(counterId);
            if (ta && counter) wire(ta, counter, fallbackMax);
        }
    };
})();