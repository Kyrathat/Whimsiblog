(function () {
    function parseMax(CharacterCounter, fallback) {
        const attr = CharacterCounter.getAttribute('maxlength')
            ?? CharacterCounter.getAttribute('data-val-length-max'); // validation
        const n = parseInt(attr, 10);
        return Number.isFinite(n) ? n : fallback;
    }

    function wire(CharacterCounter, counter, fallbackMax = 1000) {
        const max = parseMax(CharacterCounter, fallbackMax);

        function update() {
            const len = (CharacterCounter.value ?? '').length;
            counter.textContent = `${len} / ${max} characters`;
            counter.classList.toggle('text-danger', len > max);
        }

        CharacterCounter.addEventListener('input', update);
        update(); // initial render
    }

    // Public API
    window.CharacterCounterCounter = {
        initByIds: function (CharacterCounterId, counterId, fallbackMax) {
            const ta = document.getElementById(CharacterCounterId);
            const counter = document.getElementById(counterId);
            if (ta && counter) wire(ta, counter, fallbackMax);
        }
    };

    // Auto-init for this page (Description + descCounter)
    document.addEventListener('DOMContentLoaded', function () {
        const ta = document.getElementById('Description');
        const counter = document.getElementById('descCounter');
        if (ta && counter) {
            wire(ta, counter, 1000);
        }
    });
})();
