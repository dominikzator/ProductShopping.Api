document.addEventListener('submit', async function (e) {
    const cartCountValue = document.getElementById('cart-count-value');

    const form = e.target.closest('.js-cart-action-form');
    if (!form) return;

    console.log("INTERCEPTED", form.action);
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
            showFloatingAlert('error', result.message ?? 'Cart update failed.');
            return;
        }

        await refreshCartContent();

        if (cartCountValue && typeof result.cartItemsCount !== 'undefined') {
            cartCountValue.textContent = result.cartItemsCount;
        }

        if (result.message) {
            showFloatingAlert('success', result.message);
        }
    } catch (error) {
        console.error('Cart AJAX error:', error);
        showFloatingAlert('error', 'Unexpected error while updating cart.');
    }
});

async function refreshCartContent() {
    const cartContent = document.getElementById('cartContent');
    if (!cartContent) return;

    const response = await fetch('/Cart?handler=Content', {
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    });

    if (!response.ok) {
        const text = await response.text();
        throw new Error(`Failed to refresh cart content. Status: ${response.status}. Body: ${text.substring(0, 200)}`);
    }

    const html = await response.text();
    cartContent.innerHTML = html;
}