const stripe = Stripe(stripePublicKey);

initialize();

// Create a Checkout Session
function getClientSecret() {
    return new Promise(function (resolve, reject) {
        doPost({
            url : "CreateStripeClientSecret",
            formData: null,
            successCallback: (result) => {
                resolve(result.clientSecret);
            }
        });
    });
}

async function initialize() {
    const fetchClientSecret = async () => {
         return getClientSecret();
    }

    const checkout = await stripe.initEmbeddedCheckout({
        fetchClientSecret,
    });

    // Mount Checkout
    checkout.mount('#stripeCheckout');
}