(function () {
    var ta = document.getElementById('Description');
    var counter = document.getElementById('descCounter');
    if (!ta || !counter) return;

    function getMax() {
        var attr = ta.getAttribute('maxlength');
        if (attr) return parseInt(attr);
        var dataMax = ta.getAttribute('data-val-length-max');
        if (dataMax) return parseInt(dataMax);
        return 1000;
    }

    var max = getMax();

    function update() {
        var len = (ta.value || '').length;
        counter.textContent = len + ' / ' + max + ' characters';
        if (len > max) {
            counter.classList.add('text-danger');
        } else {
            counter.classList.remove('text-danger');
        }
    }

    ta.addEventListener('input', update);
    update();
})();
