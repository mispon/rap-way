mergeInto(LibraryManager.library, {

    ShowInterstitialAdv: function () {
        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onOpen: () => {
                    console.log('Show Interstitial!');
                    myGameInstance.SendMessage('GameManager', 'PauseMusic');
                },
                onClose: function(wasShown) {
                    myGameInstance.SendMessage('GameManager', 'UnpauseMusic');
                },
                onError: function(error) {
                    console.log('Error while open video ad:', e);
                }
            }
        });
    },

    ShowRewardedAdv: function () {
        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    console.log('Show Rewarded!');
                    myGameInstance.SendMessage('GameManager', 'PauseMusic');
                },
                onClose: () => {
                    myGameInstance.SendMessage('GameManager', 'UnpauseMusic');
                },
                onRewarded: () => {
                    console.log('Rewarded!');
                    myGameInstance.SendMessage('GameManager', 'OnRewardedAdCompleted');
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