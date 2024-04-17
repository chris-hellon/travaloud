const stripe = Stripe(stripePublicKey);

initialize();

// Create a Checkout Session
function getClientSecret() {
    return new Promise(function (resolve, reject) {
        postAjax("CreateStripeClientSecret", null, function (result) {
            console.log(result);
            resolve(result.clientSecret);
        });
    });
}

async function initialize() {

    const fetchClientSecret = async () => {
         return getClientSecret();
    }

    
    // const fetchClientSecret = async () => {
    //     const response = await fetch("/?handler=CreateStripeClientSecret", {
    //         method: "POST"
    //     });
    //     const { clientSecret } = await response.json();
    //    
    //     return clientSecret;
    // };

    const checkout = await stripe.initEmbeddedCheckout({
        fetchClientSecret,
    });

    // Mount Checkout
    checkout.mount('#stripeCheckout');
}