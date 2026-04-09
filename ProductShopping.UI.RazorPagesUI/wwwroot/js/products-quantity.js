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
    const dropdowns = document.querySelectorAll("[data-dropdown]");

    dropdowns.forEach(dropdown => {
        const trigger = dropdown.querySelector("[data-dropdown-trigger]");
        const menu = dropdown.querySelector("[data-dropdown-menu]");

        if (!trigger || !menu) return;

        trigger.addEventListener("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            const isOpen = dropdown.classList.contains("is-open");

            dropdowns.forEach(d => d.classList.remove("is-open"));

            if (!isOpen) {
                dropdown.classList.add("is-open");
            }
        });

        menu.addEventListener("click", function (e) {
            e.stopPropagation();
        });
    });

    document.addEventListener("click", function (e) {
        dropdowns.forEach(dropdown => {
            if (!dropdown.contains(e.target)) {
                dropdown.classList.remove("is-open");
            }
        });
    });

    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape") {
            dropdowns.forEach(dropdown => dropdown.classList.remove("is-open"));
        }
    });
});
console.log("Siemanko!");