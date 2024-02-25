using MessageBroker;
using MessageBroker.Messages.Donate;
using UniRx;

namespace Game.Player.Donate
{
    public class DonateEventsHandler : BaseEventsHandler
    {
        protected override void RegisterHandlers()
        {
            HandleAddDonate();
            HandleSpendDonate();
            HandleNoAddPurchase();
        }

        private void HandleAddDonate()
        {
            MainMessageBroker.Instance
                .Receive<AddDonateEvent>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Donate;
                    int newVal = playerData.Donate + e.Amount;

                    playerData.Donate = newVal;
                    MainMessageBroker.Instance.Publish(new DonateAddedEvent());
                    MainMessageBroker.Instance.Publish(new DonateChangedEvent {OldVal = oldVal, NewVal = newVal});
                    
                    GameManager.Instance.SaveDonateBalance();
                })
                .AddTo(disposable);
        }
        
        private void HandleSpendDonate()
        {
            MainMessageBroker.Instance
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
                        MainMessageBroker.Instance.Publish(new DonateChangedEvent {OldVal = oldVal, NewVal = newVal});
                        
                        GameManager.Instance.SaveDonateBalance();
                        ok = true;
                    } 
                    
                    MainMessageBroker.Instance.Publish(new SpendDonateResponse {OK = ok});
                })
                .AddTo(disposable);
        }

        private void HandleNoAddPurchase()
        {
            MainMessageBroker.Instance
                .Receive<NoAdsPurchaseEvent>()
                .Subscribe(_ =>
                {
                    GameManager.Instance.SaveNoAds();
                })
                .AddTo(disposable);
        }
    }
}