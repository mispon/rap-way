using Core.Interfaces;
using Game;
using MessageBroker.Messages.Donate;
using Models.Player;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public class PlayerDonateHandler : MonoBehaviour, IStarter
    {
        private IMessageBroker _messageBroker;
        
        private PlayerData _playerData;
        
        public void OnStart()
        {
            _messageBroker = GameManager.Instance.MessageBroker;
            _playerData = GameManager.Instance.PlayerData;
            
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
                    int oldVal = _playerData.Donate;
                    int newVal = _playerData.Donate + e.Amount;

                    _playerData.Donate = newVal;
                    _messageBroker.Publish(new DonateAddedEvent());
                    _messageBroker.Publish(new DonateChangedEvent {OldVal = oldVal, NewVal = newVal});
                    
                    GameManager.Instance.SaveDonateBalance();
                });
        }
        
        private void HandleSpendDonate()
        {
            _messageBroker
                .Receive<SpendDonateRequest>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Donate;
                    
                    bool ok = false;
                    if (_playerData.Donate >= e.Amount)
                    {
                        int newVal = _playerData.Donate - e.Amount;

                        _playerData.Donate = newVal;
                        _messageBroker.Publish(new DonateChangedEvent {OldVal = oldVal, NewVal = newVal});
                        
                        GameManager.Instance.SaveDonateBalance();
                        ok = true;
                    } 
                    
                    _messageBroker.Publish(new SpendDonateResponse {OK = ok});
                });
        }

        private void HandleNoAddPurchase()
        {
            _messageBroker
                .Receive<NoAdsPurchaseEvent>()
                .Subscribe(_ =>
                {
                    GameManager.Instance.SaveNoAds();
                });
        }
    }
}