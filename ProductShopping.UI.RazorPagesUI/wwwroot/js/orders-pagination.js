document.addEventListener('click', async function (event) {
    const button = event.target.closest('.js-orders-pagination-btn');
    if (!button || button.disabled || button.classList.contains('orders-pagination__btn--disabled')) {
        return;
    }

    const page = button.dataset.page;
    const container = document.getElementById('orders-list-container');

    if (!page || !container) {
        return;
    }

    const url = `/Orders?handler=OrdersList&PageNumber=${encodeURIComponent(page)}&PageSize=5`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            console.error('Failed to load orders page.');
            return;
        }

        const html = await response.text();
        container.innerHTML = html;

        window.scrollTo({
            top: document.body.scrollHeight,
            behavior: 'smooth'
        });

        window.history.replaceState({}, '', `/Orders?PageNumber=${encodeURIComponent(page)}&PageSize=5`);
    } catch (error) {
        console.error('Orders pagination error:', error);
    }
});