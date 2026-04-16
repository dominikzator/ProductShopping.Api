document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('submit', async function (e) {
        const form = e.target.closest('.js-create-order-form');
        if (!form) return;

        e.preventDefault();

        const submitButton = form.querySelector('.js-create-order-btn');
        const btnText = form.querySelector('.js-create-order-btn-text');
        const btnSpinner = form.querySelector('.js-create-order-btn-spinner');

        try {
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.classList.add('is-loading');
            }
            if (btnText && btnSpinner) {
                btnText.style.display = 'none';
                btnSpinner.style.display = 'inline-flex';
            }

            const response = await fetch(form.action || window.location.pathname, {
                method: 'POST',
                body: new FormData(form),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const result = await readJsonSafely(response);

            if (!response.ok || !result.success) {
                showFloatingAlert('error', result.message ?? 'Order creation failed.');
                return;
            }

            // komunikat
            showFloatingAlert('success', result.message ?? 'Redirecting to payment page…');

            // aktualizacja badge koszyka, jeśli backend zwróci count
            const cartCountValue = document.getElementById('cart-count-value');
            if (cartCountValue && typeof result.cartItemsCount !== 'undefined') {
                cartCountValue.textContent = result.cartItemsCount;
            }

            // redirect po krótkim czasie
            if (result.redirectUrl) {
                setTimeout(() => {
                    window.location.href = result.redirectUrl;
                }, 1200);
            }
        } catch (error) {
            console.error('Create order AJAX error:', error);
            showFloatingAlert('error', 'Unexpected error while creating order.');
        } finally {
            // jeśli redirect nastąpi, user i tak opuści stronę – to jest głównie dla przypadków błędu
            if (submitButton) {
                submitButton.disabled = false;
                submitButton.classList.remove('is-loading');
            }
            if (btnText && btnSpinner) {
                btnText.style.display = 'inline-flex';
                btnSpinner.style.display = 'none';
            }
        }
    });
});