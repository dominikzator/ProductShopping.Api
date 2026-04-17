document.addEventListener('click', async function (event) {
    const button = event.target.closest('.js-pagination-link');
    if (!button || button.disabled || button.classList.contains('is-disabled')) {
        return;
    }

    const page = button.dataset.page;
    const container = document.getElementById('products-list-container');
    const filtersForm = document.querySelector('.filters-toolbar');

    if (!page || !container || !filtersForm) {
        return;
    }

    const formData = new FormData(filtersForm);
    const params = new URLSearchParams();

    for (const [key, value] of formData.entries()) {
        if (value !== null && value !== undefined && value !== '') {
            params.set(key, value.toString());
        }
    }

    params.set('Query.PageNumber', page);

    try {
        const response = await fetch(`/Products?handler=ProductsList&${params.toString()}`, {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            console.error('Failed to load products list.');
            return;
        }

        const html = await response.text();
        container.innerHTML = html;

        const newUrl = `/Products?${params.toString()}`;
        window.history.replaceState({}, '', newUrl);
    } catch (error) {
        console.error('Pagination error:', error);
    }
});