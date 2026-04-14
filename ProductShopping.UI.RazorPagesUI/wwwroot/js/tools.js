function showFloatingAlert(type, message) {
    let layer = document.getElementById('pageAlertLayer');

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

async function readJsonSafely(response) {
    const contentType = response.headers.get('content-type') || '';
    const text = await response.text();

    if (!contentType.includes('application/json')) {
        throw new Error(`Expected JSON, got: ${contentType}. Body: ${text.substring(0, 200)}`);
    }

    return JSON.parse(text);
}