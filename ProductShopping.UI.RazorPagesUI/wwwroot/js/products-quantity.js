document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.quantity-control').forEach(function (wrapper) {
        const input = wrapper.querySelector('.qty-input');
        const minus = wrapper.querySelector('.qty-btn--minus');
        const plus = wrapper.querySelector('.qty-btn--plus');

        const min = parseInt(input.min || '1', 10);
        const max = parseInt(input.max || '999', 10);

        minus.addEventListener('click', function () {
            let value = parseInt(input.value || '0', 10);
            if (isNaN(value)) value = min;
            value = Math.max(min, value - 1);
            input.value = value;
        });

        plus.addEventListener('click', function () {
            let value = parseInt(input.value || '0', 10);
            if (isNaN(value)) value = min;
            value = Math.max(min, Math.min(max, value + 1));
            input.value = value;
        });

        input.addEventListener('input', function () {
            let value = parseInt(input.value || '0', 10);
            if (isNaN(value) || value < min) value = min;
            if (value > max) value = max;
            input.value = value;
        });
    });
});
console.log("Siemanko!");