document.addEventListener('DOMContentLoaded', function () {
    const dropdowns = document.querySelectorAll('[data-dropdown]');
    const cartCountValue = document.getElementById('cart-count-value');
    const alertLayer = document.getElementById('pageAlertLayer');

    async function readJsonSafely(response) {
        const contentType = response.headers.get('content-type') || '';

        if (!contentType.includes('application/json')) {
            return {};
        }

        try {
            return await response.json();
        } catch {
            return {};
        }
    }

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
        const minus = e.target.closest('.qty-btn--minus');
        const plus = e.target.closest('.qty-btn--plus');
        const closeAlertButton = e.target.closest('[data-alert-close]');

        if (minus) {
            const wrapper = minus.closest('.quantity-control');
            const input = wrapper?.querySelector('.qty-input');

            if (!input) return;

            const min = parseInt(input.min || '1', 10);
            let value = parseInt(input.value || '0', 10);

            if (isNaN(value)) value = min;
            value = Math.max(min, value - 1);
            input.value = value;
            return;
        }

        if (plus) {
            const wrapper = plus.closest('.quantity-control');
            const input = wrapper?.querySelector('.qty-input');

            if (!input) return;

            const min = parseInt(input.min || '1', 10);
            const max = parseInt(input.max || '999', 10);
            let value = parseInt(input.value || '0', 10);

            if (isNaN(value)) value = min;
            value = Math.max(min, Math.min(max, value + 1));
            input.value = value;
            return;
        }

        if (closeAlertButton) {
            const alert = closeAlertButton.closest('.page-alert');
            if (!alert) return;

            alert.classList.add('is-hiding');
            setTimeout(() => {
                alert.remove();
            }, 450);
            return;
        }

        dropdowns.forEach(dropdown => {
            if (!dropdown.contains(e.target)) {
                dropdown.classList.remove('is-open');
            }
        });
    });

    document.addEventListener('input', function (e) {
        const input = e.target.closest('.qty-input');
        if (!input) return;

        const min = parseInt(input.min || '1', 10);
        const max = parseInt(input.max || '999', 10);
        let value = parseInt(input.value || '0', 10);

        if (isNaN(value) || value < min) value = min;
        if (value > max) value = max;

        input.value = value;
    });

    document.addEventListener('submit', async function (e) {
        const form = e.target.closest('.js-add-to-cart-form');
        if (!form) return;

        e.preventDefault();

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const result = await readJsonSafely(response);

            if (!response.ok || !result.success) {
                showFloatingAlert('error', result.message ?? 'Failed to add product to cart.');
                return;
            }

            if (cartCountValue && typeof result.cartItemsCount !== 'undefined') {
                cartCountValue.textContent = result.cartItemsCount;
            }

            showFloatingAlert('success', result.message ?? 'Product added to cart.');
        } catch (error) {
            console.error('Add to cart error:', error);
            showFloatingAlert('error', 'Unexpected error while adding product to cart.');
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            dropdowns.forEach(dropdown => dropdown.classList.remove('is-open'));
        }
    });
});