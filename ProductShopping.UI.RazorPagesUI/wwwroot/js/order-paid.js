document.addEventListener('DOMContentLoaded', async function () {
    const params = new URLSearchParams(window.location.search);
    const sessionId = params.get('session_id');

    if (!sessionId) {
        console.error('Missing session_id in URL.');
        return;
    }

    try {
        const response = await fetch(`/api/payments/success?session_id=${encodeURIComponent(sessionId)}`, {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const result = await readJsonSafely(response);

        if (!response.ok || !result.success) {
            console.error('Payment status request failed:', result.message ?? result);
            return;
        }

        const targetUrl = new URL('/OrderPaid', window.location.origin);
        targetUrl.searchParams.set('session_id', result.sessionId);

        if (result.orderId) {
            targetUrl.searchParams.set('orderId', result.orderId);
        }

        console.log('Target redirect URL:', targetUrl.toString());
        console.log('API result:', result);
    } catch (error) {
        console.error('Order paid page error:', error);
    }
});

console.log("Order Paid Loaded!");