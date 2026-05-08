function showFloatingAlert(type, message, closeOnTime = true) {
    let layer = document.getElementById('pageAlertLayer');

    if (!layer) {
        layer = document.createElement('div');
        layer.id = 'pageAlertLayer';
        layer.className = 'page-alert-layer';
        document.body.appendChild(layer);
    }

    if (layer._alertTimeoutId) {
        clearTimeout(layer._alertTimeoutId);
        layer._alertTimeoutId = null;
    }

    const alertId = `${Date.now()}-${Math.random().toString(36).slice(2)}`;
    layer.dataset.alertId = alertId;

    layer.innerHTML = closeOnTime
        ? `
            <div class="page-alert page-alert--${type}" role="alert" data-alert-id="${alertId}">
                <span class="page-alert-text"></span>
                <button type="button" class="page-alert-close" data-alert-close aria-label="Zamknij komunikat">×</button>
            </div>
          `
        : `
            <div class="page-alert page-alert--${type}" role="alert" data-alert-id="${alertId}">
                <span class="page-alert-text"></span>
            </div>
          `;

    const alert = layer.querySelector(`.page-alert[data-alert-id="${alertId}"]`);
    const alertText = alert?.querySelector('.page-alert-text');
    const closeButton = alert?.querySelector('[data-alert-close]');

    if (!alert || !alertText) return;

    alertText.textContent = message;

    const hideAlert = () => {
        if (layer.dataset.alertId !== alertId) return;
        if (!alert.isConnected) return;

        alert.classList.add('is-hiding');

        setTimeout(() => {
            if (layer.dataset.alertId !== alertId) return;

            if (alert.isConnected) {
                alert.remove();
            }

            if (layer.dataset.alertId === alertId) {
                layer.innerHTML = '';
                delete layer.dataset.alertId;
            }
        }, 450);

        if (layer._alertTimeoutId) {
            clearTimeout(layer._alertTimeoutId);
            layer._alertTimeoutId = null;
        }
    };

    alert.classList.remove('is-entering', 'is-hiding');
    void alert.offsetWidth;
    alert.classList.add('is-entering');

    if (closeOnTime) {
        layer._alertTimeoutId = setTimeout(() => {
            if (layer.dataset.alertId !== alertId) return;
            hideAlert();
        }, type === 'error' ? 3000 : 2000);
    }

    if (closeButton) {
        closeButton.addEventListener('click', () => {
            if (layer.dataset.alertId !== alertId) return;
            hideAlert();
        }, { once: true });
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