using Core.OrderedStarter;
using Game;
using MessageBroker.Messages.Donate;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public class PlayerDonateHandler : MonoBehaviour, IStarter
    {
        private readonly CompositeDisposable _disposable = new(); 
        
        public void OnStart()
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
                .AddTo(_disposable);
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
                .AddTo(_disposable);
        }

        private void HandleNoAddPurchase()
        {
            MainMessageBroker.Instance
                .Receive<NoAdsPurchaseEvent>()
                .Subscribe(_ =>
                {
                    GameManager.Instance.SaveNoAds();
                })
                .AddTo(_disposable);
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}