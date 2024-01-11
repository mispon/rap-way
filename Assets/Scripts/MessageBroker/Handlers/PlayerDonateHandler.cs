using System;
using Core.Interfaces;
using Game;
using MessageBroker.Messages.Donate;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public class PlayerDonateHandler : MonoBehaviour, IStarter
    {
        private readonly CompositeDisposable _disposable = new(); 
        
        private IMessageBroker _messageBroker;
        
        public void OnStart()
        {
            _messageBroker = GameManager.Instance.MessageBroker;
            
            HandleAddDonate();
            HandleSpendDonate();
            HandleNoAddPurchase();
        }
        
        private void HandleAddDonate()
        {
            _messageBroker
                .Receive<AddDonateEvent>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Donate;
                    int newVal = playerData.Donate + e.Amount;

                    playerData.Donate = newVal;
                    _messageBroker.Publish(new DonateAddedEvent());
                    _messageBroker.Publish(new DonateChangedEvent {OldVal = oldVal, NewVal = newVal});
                    
                    GameManager.Instance.SaveDonateBalance();
                })
                .AddTo(_disposable);
        }
        
        private void HandleSpendDonate()
        {
            _messageBroker
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
                        _messageBroker.Publish(new DonateChangedEvent {OldVal = oldVal, NewVal = newVal});
                        
                        GameManager.Instance.SaveDonateBalance();
                        ok = true;
                    } 
                    
                    _messageBroker.Publish(new SpendDonateResponse {OK = ok});
                })
                .AddTo(_disposable);
        }

        private void HandleNoAddPurchase()
        {
            _messageBroker
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