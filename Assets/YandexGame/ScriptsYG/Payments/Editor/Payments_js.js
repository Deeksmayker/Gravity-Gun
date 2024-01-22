
var paymentsData = null;

function GetPayments(sendback) {
    return new Promise((resolve, reject) => {
        try {
            ysdk.getPayments().then(_payments => {
                console.log('Purchases are available');
                payments = _payments;

                payments.getCatalog()
                    .then(products => {
                        let productID = [];
                        let title = [];
                        let description = [];
                        let imageURI = [];
                        let priceValue = [];
                        let consumed = [];

                        payments.getPurchases().then(purchases => {
                            for (let i = 0; i < products.length; i++) {
                                productID[i] = products[i].id;
                                title[i] = products[i].title;
                                description[i] = products[i].description;
                                imageURI[i] = products[i].imageURI;
                                priceValue[i] = products[i].priceValue;

                                consumed[i] = true;
                                for (i2 = 0; i2 < purchases.length; i2++) {
                                    if (purchases[i2].productID === productID[i]) {
                                        consumed[i] = false;
                                        break;
                                    }
                                }
                            }

                            let jsonPayments = {
                                "id": productID,
                                "title": title,
                                "description": description,
                                "imageURI": imageURI,
                                "priceValue": priceValue,
                                "consumed": consumed,
                                "language": ysdk.environment.i18n.lang
                            };

                            if (sendback)
                                myGameInstance.SendMessage('YandexGame', 'PaymentsEntries', JSON.stringify(jsonPayments));
                            resolve(JSON.stringify(jsonPayments));
                        });
                    });

            }).catch(e => {
                console.log('Purchases are not available', e.message);
            })
        } catch (e) {
            console.error('CRASH Init Payments: ', e.message);
            reject(e);
        }
    });
}