using MessageBroker;
using MessageBroker.Interfaces;
using MessageBroker.Messages.Player;
using UniRx;

namespace Game.Player.Donate
{
    public class DonateMessagesHandler : IMessagesHandler
    {
        public void RegisterHandlers(CompositeDisposable disposable)
        {
            HandleAddDonate(disposable);
            HandleSpendDonate(disposable);
            HandleNoAddPurchase(disposable);
        }

        private static void HandleAddDonate(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<AddDonateMessage>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Donate;
                    int newVal = playerData.Donate + e.Amount;

                    playerData.Donate = newVal;
                    MsgBroker.Instance.Publish(new DonateAddedMessage());
                    MsgBroker.Instance.Publish(new DonateChangedMessage {OldVal = oldVal, NewVal = newVal});
                    
                    GameManager.Instance.SaveDonateBalance();
                })
                .AddTo(disposable);
        }
        
        private static void HandleSpendDonate(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<SpendDonateRequest>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Donate;
                    
                    bool ok = false;
                    if (playerData.Donate >= e.Amount)
                    {
                        int newVal = playerData.Donate - e.Amount;

                        playerData.Donate = newVal;
                        MsgBroker.Instance.Publish(new DonateChangedMessage {OldVal = oldVal, NewVal = newVal});
                        
                        GameManager.Instance.SaveDonateBalance();
                        ok = true;
                    } 
                    
                    MsgBroker.Instance.Publish(new SpendDonateResponse {OK = ok});
                })
                .AddTo(disposable);
        }

        private static void HandleNoAddPurchase(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<NoAdsPurchaseMessage>()
                .Subscribe(_ =>
                {
                    GameManager.Instance.SaveNoAds();
                })
                .AddTo(disposable);
        }
    }
}