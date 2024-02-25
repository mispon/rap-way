using MessageBroker;
using MessageBroker.Handlers;
using MessageBroker.Messages.Player;
using UniRx;

namespace Game.Player.Donate
{
    public class DonateMessagesHandler : BaseMessagesHandler
    {
        protected override void RegisterHandlers()
        {
            HandleAddDonate();
            HandleSpendDonate();
            HandleNoAddPurchase();
        }

        private void HandleAddDonate()
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
        
        private void HandleSpendDonate()
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

        private void HandleNoAddPurchase()
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