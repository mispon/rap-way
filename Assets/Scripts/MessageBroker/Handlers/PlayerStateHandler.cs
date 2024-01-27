using System.Linq;
using Core.OrderedStarter;
using Game;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.State;
using ScriptableObjects;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public class PlayerStateHandler : MonoBehaviour, IStarter
    {
        private readonly CompositeDisposable _disposable = new(); 
        
        private GameSettings _settings;
        
        public void OnStart()
        {
            _settings = GameManager.Instance.Settings;

            HandleFullStateRequest();
            HandleSpendMoneyRequest();
            HandleChangeMoney();
            HandleChangeFans();
            HandleChangeHype();
            HandleChangeExp();
            HandleProductionReward();
            HandleConvertReward();
        }

        private void HandleFullStateRequest()
        {
            MainMessageBroker.Instance
                .Receive<FullStateRequest>()
                .Subscribe(_ =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    MainMessageBroker.Instance.Publish(new FullStateResponse
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
        
        private void HandleChangeMoney()
        {
            MainMessageBroker.Instance
                .Receive<ChangeMoneyEvent>()
                .Subscribe(e => UpdateMoney(e.Amount))
                .AddTo(_disposable);
        }
        
        private void HandleSpendMoneyRequest()
        {
            MainMessageBroker.Instance
                .Receive<SpendMoneyRequest>()
                .Subscribe(e =>
                {
                    bool isMoneyEnough = GameManager.Instance.PlayerData.Money >= e.Amount;
                    
                    if (isMoneyEnough)
                        UpdateMoney(-e.Amount);
                    
                    MainMessageBroker.Instance.Publish(new SpendMoneyResponse {OK = isMoneyEnough});
                })
                .AddTo(_disposable);
        }

        private void HandleChangeFans()
        {
            MainMessageBroker.Instance
                .Receive<ChangeFansEvent>()
                .Subscribe(e => UpdateFans(e.Amount))
                .AddTo(_disposable);
        }
        
        private void HandleChangeHype()
        {
            MainMessageBroker.Instance
                .Receive<ChangeHypeEvent>()
                .Subscribe(e => UpdateHype(e.Amount))
                .AddTo(_disposable);
        }
        
        private void HandleChangeExp()
        {
            MainMessageBroker.Instance
                .Receive<ChangeExpEvent>()
                .Subscribe(e => UpdateExp(e.Amount))
                .AddTo(_disposable);
        }

        private void HandleProductionReward()
        {
            MainMessageBroker.Instance
                .Receive<ProductionRewardEvent>()
                .Subscribe(e =>
                {
                    UpdateMoney(e.MoneyIncome);
                    UpdateFans(e.FansIncome);
                    UpdateHype(e.HypeIncome);
                    UpdateExp(e.Exp);
                    
                    if (e.WithSocialCooldown)
                        GameManager.Instance.GameStats.SocialsCooldown = _settings.SocialsCooldown;
                    
                    GameManager.Instance.SaveApplicationData();
                })
                .AddTo(_disposable);
        }

        private void HandleConvertReward()
        {
            MainMessageBroker.Instance
                .Receive<ConcertRewardEvent>()
                .Subscribe(e =>
                {
                    UpdateMoney(e.MoneyIncome);
                    GameManager.Instance.GameStats.ConcertCooldown = _settings.ConcertCooldown;
                    GameManager.Instance.SaveApplicationData();
                })
                .AddTo(_disposable);
        }

        private void UpdateMoney(int value)
        {
            var playerData = GameManager.Instance.PlayerData;
                    
            int oldVal = playerData.Money;
            int newVal = SafetyAdd(oldVal, value, _settings.MaxMoney);

            playerData.Money = newVal;
            MainMessageBroker.Instance.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
        }

        private void UpdateFans(int value)
        {
            var playerData = GameManager.Instance.PlayerData;
                    
            int oldVal = playerData.Fans;
            int newVal = SafetyAdd(oldVal, value, _settings.MaxFans);
                    
            const int minFans = 0;
            newVal = Mathf.Max(newVal, minFans);
                    
            playerData.Fans = newVal;
            MainMessageBroker.Instance.Publish(new FansChangedEvent {OldVal = oldVal, NewVal = newVal});
        }

        private void UpdateExp(int value)
        {
            var playerData = GameManager.Instance.PlayerData;
                    
            int oldVal = playerData.Exp;
            int newVal = playerData.Exp + value;
                    
            const int minExp = 0;
            newVal = Mathf.Max(newVal, minExp);

            playerData.Exp = newVal;
            MainMessageBroker.Instance.Publish(new ExpChangedEvent {OldVal = oldVal, NewVal = newVal});
        }

        private void UpdateHype(int value)
        {
            var playerData = GameManager.Instance.PlayerData;
                    
            int oldVal = playerData.Hype;
            int newVal = playerData.Hype + value;

            int minHype = playerData.Goods
                .GroupBy(g => g.Type)
                .ToDictionary(k => k, v => v.Max(g => g.Hype))
                .Sum(g => g.Value);
            const int maxHype = 100;
                    
            newVal = Mathf.Clamp(newVal, minHype, maxHype);

            playerData.Hype = newVal;
            MainMessageBroker.Instance.Publish(new HypeChangedEvent {OldVal = oldVal, NewVal = newVal});
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