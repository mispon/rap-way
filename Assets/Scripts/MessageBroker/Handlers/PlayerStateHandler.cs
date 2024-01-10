using System.Linq;
using Core.Interfaces;
using Core.Settings;
using Game;
using MessageBroker.Messages.State;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public class PlayerStateHandler : MonoBehaviour, IStarter
    {
        private readonly CompositeDisposable _disposable = new(); 
        
        private IMessageBroker _messageBroker;
        
        private GameSettings _settings;
        
        public void OnStart()
        {
            _messageBroker = GameManager.Instance.MessageBroker;
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
                    var playerData = GameManager.Instance.PlayerData;
                    
                    _messageBroker.Publish(new FullStateResponse
                    {
                        NickName = playerData.Info.NickName,
                        Gender = playerData.Info.Gender,
                        Money = playerData.Money,
                        Donate = playerData.Donate,
                        Fans = playerData.Fans,
                        Hype = playerData.Hype,
                        Exp = playerData.Exp
                    });
                })
                .AddTo(_disposable);
        }
        
        private void HandleAddMoney()
        {
            _messageBroker
                .Receive<AddMoneyEvent>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Money;
                    int newVal = SafetyAdd(playerData.Money, e.Amount, _settings.MaxMoney);

                    playerData.Money = newVal;
                    _messageBroker.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
                })
                .AddTo(_disposable);
        }
        
        private void HandleSpendMoney()
        {
            _messageBroker
                .Receive<SpendMoneyRequest>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Money;
                    
                    bool ok = false;
                    if (playerData.Money >= e.Amount)
                    {
                        int newVal = playerData.Money - e.Amount;

                        playerData.Money = newVal;
                        _messageBroker.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
                        
                        ok = true;
                    } 
                    
                    _messageBroker.Publish(new SpendMoneyResponse {OK = ok});
                })
                .AddTo(_disposable);
        }

        private void HandleChangeFans()
        {
            _messageBroker
                .Receive<ChangeFansEvent>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Fans;
                    int newVal = SafetyAdd(playerData.Money, e.Amount, _settings.MaxFans);
                    
                    const int minFans = 0;
                    newVal = Mathf.Max(newVal, minFans);
                    
                    playerData.Fans = newVal;
                    _messageBroker.Publish(new FansChangedEvent {OldVal = oldVal, NewVal = newVal});
                })
                .AddTo(_disposable);
        }
        
        private void HandleChangeHype()
        {
            _messageBroker
                .Receive<ChangeHypeEvent>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Hype;
                    int newVal = playerData.Hype + e.Amount;

                    int minHype = playerData.Goods
                        .GroupBy(g => g.Type)
                        .ToDictionary(k => k, v => v.Max(g => g.Hype))
                        .Sum(g => g.Value);
                    const int maxHype = 100;
                    
                    newVal = Mathf.Clamp(newVal, minHype, maxHype);

                    playerData.Hype = newVal;
                    _messageBroker.Publish(new HypeChangedEvent {OldVal = oldVal, NewVal = newVal});
                })
                .AddTo(_disposable);
        }
        
        private void HandleChangeExp()
        {
            _messageBroker
                .Receive<ChangeExpEvent>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    int oldVal = playerData.Exp;
                    int newVal = playerData.Exp + e.Amount;
                    
                    const int minExp = 0;
                    newVal = Mathf.Max(newVal, minExp);

                    playerData.Exp = newVal;
                    _messageBroker.Publish(new ExpChangedEvent {OldVal = oldVal, NewVal = newVal});
                })
                .AddTo(_disposable);
        }
        
        private static int SafetyAdd(int current, int increment, int maxValue)
        {
            return maxValue - current > increment
                ? current + increment
                : maxValue;
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}