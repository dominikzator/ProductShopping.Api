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

    const dropdowns = document.querySelectorAll('[data-dropdown]');

    dropdowns.forEach(dropdown => {
        const trigger = dropdown.querySelector('[data-dropdown-trigger]');
        const menu = dropdown.querySelector('[data-dropdown-menu]');

        if (!trigger || !menu) return;

        trigger.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const isOpen = dropdown.classList.contains('is-open');

            dropdowns.forEach(d => d.classList.remove('is-open'));

            if (!isOpen) {
                dropdown.classList.add('is-open');
            }
        });

        menu.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    });

    document.addEventListener('click', function (e) {
        dropdowns.forEach(dropdown => {
            if (!dropdown.contains(e.target)) {
                dropdown.classList.remove('is-open');
            }
        });
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            dropdowns.forEach(dropdown => dropdown.classList.remove('is-open'));
        }
    });

    const existingAlert = document.querySelector("[data-auto-dismiss='true']");
    if (existingAlert) {
        const closeButton = existingAlert.querySelector('[data-alert-close]');

        const hideAlert = () => {
            existingAlert.classList.add('is-hiding');
            setTimeout(() => {
                existingAlert.remove();
            }, 450);
        };

        const autoHideTimeout = setTimeout(hideAlert, 2000);

        if (closeButton) {
            closeButton.addEventListener('click', function () {
                clearTimeout(autoHideTimeout);
                hideAlert();
            });
        }
    }

    const addToCartForms = document.querySelectorAll('.js-add-to-cart-form');
    const alertLayer = document.getElementById('pageAlertLayer');

    function showFloatingAlert(type, message) {
        let layer = alertLayer;

        if (!layer) {
            layer = document.createElement('div');
            layer.id = 'pageAlertLayer';
            layer.className = 'page-alert-layer';
            document.body.appendChild(layer);
        }

        layer.innerHTML = `
            <div class="page-alert page-alert--${type}" data-auto-dismiss="true" role="alert">
                <span class="page-alert-text">${message}</span>
                <button type="button" class="page-alert-close" data-alert-close aria-label="Zamknij komunikat">×</button>
            </div>
        `;

        const alert = layer.querySelector('.page-alert');
        const closeButton = layer.querySelector('[data-alert-close]');

        const hideAlert = () => {
            if (!alert) return;
            alert.classList.add('is-hiding');
            setTimeout(() => {
                layer.innerHTML = '';
            }, 450);
        };

        const autoHideTimeout = setTimeout(hideAlert, type === 'error' ? 3000 : 2000);

        if (closeButton) {
            closeButton.addEventListener('click', function () {
                clearTimeout(autoHideTimeout);
                hideAlert();
            });
        }
    }

    const cartCountValue = document.getElementById("cart-count-value");

    addToCartForms.forEach(form => {
        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            const formData = new FormData(form);

            try {
                const response = await fetch(form.action, {
                    method: "POST",
                    body: formData,
                    headers: {
                        "X-Requested-With": "XMLHttpRequest"
                    }
                });

                const result = await response.json();

                if (!response.ok || !result.success) {
                    showFloatingAlert("error", result.message ?? "Failed to add product to cart.");
                    return;
                }

                if (cartCountValue && typeof result.cartItemsCount !== "undefined") {
                    cartCountValue.textContent = result.cartItemsCount;
                }

                showFloatingAlert("success", result.message ?? "Product added to cart.");
            } catch (error) {
                console.error(error);
                showFloatingAlert("error", "Unexpected error while adding product to cart.");
            }
        });
    });
});