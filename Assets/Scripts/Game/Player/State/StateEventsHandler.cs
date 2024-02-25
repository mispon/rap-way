using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.State;
using UniRx;
using UnityEngine;

namespace Game.Player.State
{
    public class StateEventsHandler : BaseEventsHandler
    {
        private GameSettings _settings;

        protected override void RegisterHandlers()
        {
            _settings = GameManager.Instance.Settings;

            HandleFullStateRequest();
            HandleSpendMoneyRequest();
            HandleChangeMoney();
            HandleChangeFans();
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
                .AddTo(disposable);
        }
        
        private void HandleChangeMoney()
        {
            MainMessageBroker.Instance
                .Receive<ChangeMoneyEvent>()
                .Subscribe(e => UpdateMoney(e.Amount))
                .AddTo(disposable);
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
                .AddTo(disposable);
        }

        private void HandleChangeFans()
        {
            MainMessageBroker.Instance
                .Receive<ChangeFansEvent>()
                .Subscribe(e => UpdateFans(e.Amount))
                .AddTo(disposable);
        }
        
        private void HandleChangeExp()
        {
            MainMessageBroker.Instance
                .Receive<ChangeExpEvent>()
                .Subscribe(e => UpdateExp(e.Amount))
                .AddTo(disposable);
        }

        private void HandleProductionReward()
        {
            MainMessageBroker.Instance
                .Receive<ProductionRewardEvent>()
                .Subscribe(e =>
                {
                    UpdateMoney(e.MoneyIncome);
                    UpdateFans(e.FansIncome);
                    UpdateExp(e.Exp);
                    
                    if (e.WithSocialCooldown)
                        GameManager.Instance.GameStats.SocialsCooldown = _settings.Socials.Cooldown;
                    
                    GameManager.Instance.SaveApplicationData();
                })
                .AddTo(disposable);
        }

        private void HandleConvertReward()
        {
            MainMessageBroker.Instance
                .Receive<ConcertRewardEvent>()
                .Subscribe(e =>
                {
                    UpdateMoney(e.MoneyIncome);
                    GameManager.Instance.GameStats.ConcertCooldown = _settings.Concert.Cooldown;
                    GameManager.Instance.SaveApplicationData();
                })
                .AddTo(disposable);
        }

        private void UpdateMoney(int value)
        {
            var playerData = GameManager.Instance.PlayerData;
                    
            int oldVal = playerData.Money;
            int newVal = SafetyAdd(oldVal, value, _settings.Player.MaxMoney);

            playerData.Money = newVal;
            MainMessageBroker.Instance.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
        }

        private void UpdateFans(int value)
        {
            var playerData = GameManager.Instance.PlayerData;
                    
            int oldVal = playerData.Fans;
            int newVal = SafetyAdd(oldVal, value, _settings.Player.MaxFans);
                    
            const int minFans = 0;
            newVal = Mathf.Max(newVal, minFans);
                    
            playerData.Fans = newVal;
            MainMessageBroker.Instance.Publish(new FansChangedEvent {OldVal = oldVal, NewVal = newVal});
        }

        private static void UpdateExp(int value)
        {
            var playerData = GameManager.Instance.PlayerData;
                    
            int oldVal = playerData.Exp;
            int newVal = playerData.Exp + value;
                    
            const int minExp = 0;
            newVal = Mathf.Max(newVal, minExp);

            playerData.Exp = newVal;
            MainMessageBroker.Instance.Publish(new ExpChangedEvent {OldVal = oldVal, NewVal = newVal});
        }
        
        private static int SafetyAdd(int current, int increment, int maxValue)
        {
            return maxValue - current > increment
                ? current + increment
                : maxValue;
        }
    }
}