mergeInto(LibraryManager.library, {

    ShowInterstitialAdv: function () {
        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onClose: function(wasShown) {
                    // Действие после закрытия рекламы.
                },
                onError: function(error) {
                    // Действие в случае ошибки.
                }
            }
        });
    },

    ShowRewardedAdv: function () {
        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    console.log('Video ad open.');
                },
                onRewarded: () => {
                    console.log('Rewarded!');
                },
                onClose: () => {
                    console.log('Video ad closed.');
                },
                onError: (e) => {
                    console.log('Error while open video ad:', e);
                }
            }
        });
    },

    YandexRateGame: function () {
        ysdk.feedback.canReview()
            .then(({ value, reason }) => {
                if (value) {
                    ysdk.feedback.requestReview()
                        .then(({ feedbackSent }) => {
                            console.log(feedbackSent);
                        })
                } else {
                    console.log(reason)
                }
            });
    },

});