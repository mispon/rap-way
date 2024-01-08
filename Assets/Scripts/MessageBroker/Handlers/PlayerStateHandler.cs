using System.Linq;
using Core.Interfaces;
using Core.Settings;
using Game;
using MessageBroker.Messages.State;
using Models.Player;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public class PlayerStateHandler : MonoBehaviour, IStarter
    {
        private IMessageBroker _messageBroker;
        
        private PlayerData _playerData;
        private GameSettings _settings;
        
        public void OnStart()
        {
            _messageBroker = GameManager.Instance.MessageBroker;
            _playerData = GameManager.Instance.PlayerData;
            _settings = GameManager.Instance.Settings;

            HandleFullState();
            HandleAddMoney();
            HandleSpendMoney();
            HandleChangeFans();
            HandleChangeHype();
            HandleChangeExp();
        }

        private void HandleFullState()
        {
            _messageBroker
                .Receive<FullStateRequest>()
                .Subscribe(_ =>
                {
                    _messageBroker.Publish(new FullStateResponse
                    {
                        NickName = _playerData.Info.NickName,
                        Gender = _playerData.Info.Gender,
                        Money = _playerData.Money,
                        Donate = _playerData.Donate,
                        Fans = _playerData.Fans,
                        Hype = _playerData.Hype,
                        Exp = _playerData.Exp
                    });
                });
        }
        
        private void HandleAddMoney()
        {
            _messageBroker
                .Receive<AddMoneyEvent>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Money;
                    int newVal = SafetyAdd(_playerData.Money, e.Amount, _settings.MaxMoney);

                    _playerData.Money = newVal;
                    _messageBroker.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
                });
        }
        
        private void HandleSpendMoney()
        {
            _messageBroker
                .Receive<SpendMoneyRequest>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Money;
                    
                    bool ok = false;
                    if (_playerData.Money >= e.Amount)
                    {
                        int newVal = _playerData.Money - e.Amount;

                        _playerData.Money = newVal;
                        _messageBroker.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
                        
                        ok = true;
                    } 
                    
                    _messageBroker.Publish(new SpendMoneyResponse {OK = ok});
                });
        }

        private void HandleChangeFans()
        {
            _messageBroker
                .Receive<ChangeFansEvent>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Fans;
                    int newVal = SafetyAdd(_playerData.Money, e.Amount, _settings.MaxFans);
                    
                    const int minFans = 0;
                    newVal = Mathf.Max(newVal, minFans);
                    
                    _playerData.Fans = newVal;
                    _messageBroker.Publish(new FansChangedEvent {OldVal = oldVal, NewVal = newVal});
                });
        }
        
        private void HandleChangeHype()
        {
            _messageBroker
                .Receive<ChangeHypeEvent>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Hype;
                    int newVal = _playerData.Hype + e.Amount;

                    int minHype = _playerData.Goods
                        .GroupBy(g => g.Type)
                        .ToDictionary(k => k, v => v.Max(g => g.Hype))
                        .Sum(g => g.Value);
                    const int maxHype = 100;
                    
                    newVal = Mathf.Clamp(newVal, minHype, maxHype);

                    _playerData.Hype = newVal;
                    _messageBroker.Publish(new HypeChangedEvent {OldVal = oldVal, NewVal = newVal});
                });
        }
        
        private void HandleChangeExp()
        {
            _messageBroker
                .Receive<ChangeExpEvent>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Exp;
                    int newVal = _playerData.Exp + e.Amount;
                    
                    const int minExp = 0;
                    newVal = Mathf.Max(newVal, minExp);

                    _playerData.Exp = newVal;
                    _messageBroker.Publish(new ExpChangedEvent {OldVal = oldVal, NewVal = newVal});
                });
        }
        
        private static int SafetyAdd(int current, int increment, int maxValue)
        {
            return maxValue - current > increment
                ? current + increment
                : maxValue;
        }
    }
}