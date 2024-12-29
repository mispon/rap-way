using Game.Settings;
using MessageBroker;
using MessageBroker.Interfaces;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.Production;
using UniRx;
using UnityEngine;

namespace Game.Player.State
{
    public class StateMessagesHandler : IMessagesHandler
    {
        private GameSettings _settings;

        public void RegisterHandlers(CompositeDisposable disposable)
        {
            _settings = GameManager.Instance.Settings;

            HandleFullStateRequest(disposable);
            HandleSpendMoneyRequest(disposable);
            HandleChangeMoney(disposable);
            HandleChangeFans(disposable);
            HandleChangeExp(disposable);
            HandleProductionReward(disposable);
            HandleConvertReward(disposable);
        }

        private static void HandleFullStateRequest(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<FullStateRequest>()
                .Subscribe(_ =>
                {
                    var playerData = GameManager.Instance.PlayerData;

                    MsgBroker.Instance.Publish(new FullStateResponse
                    {
                        RealName = $"{playerData.Info.FirstName} {playerData.Info.LastName}",
                        NickName = playerData.Info.NickName,
                        Gender   = playerData.Info.Gender,
                        Money    = playerData.Money,
                        Donate   = playerData.Donate,
                        Fans     = playerData.Fans,
                        Hype     = playerData.Hype,
                        Exp      = playerData.Exp,
                        Level    = 0 // todo
                    });
                })
                .AddTo(disposable);
        }

        private void HandleChangeMoney(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<ChangeMoneyMessage>()
                .Subscribe(e => UpdateMoney(e.Amount))
                .AddTo(disposable);
        }

        private void HandleSpendMoneyRequest(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<SpendMoneyRequest>()
                .Subscribe(e =>
                {
                    var isMoneyEnough = GameManager.Instance.PlayerData.Money >= e.Amount;

                    if (isMoneyEnough)
                    {
                        UpdateMoney(-e.Amount);
                    }

                    MsgBroker.Instance.Publish(new SpendMoneyResponse {Source = e.Source, OK = isMoneyEnough});
                })
                .AddTo(disposable);
        }

        private void HandleChangeFans(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<ChangeFansMessage>()
                .Subscribe(e => UpdateFans(e.Amount))
                .AddTo(disposable);
        }

        private static void HandleChangeExp(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<ChangeExpMessage>()
                .Subscribe(e => UpdateExp(e.Amount))
                .AddTo(disposable);
        }

        private void HandleProductionReward(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<ProductionRewardMessage>()
                .Subscribe(e =>
                {
                    UpdateMoney(e.MoneyIncome);
                    UpdateFans(e.FansIncome);
                    UpdateExp(e.Exp);

                    if (e.WithSocialCooldown)
                    {
                        GameManager.Instance.GameStats.SocialsCooldown = _settings.Socials.Cooldown;
                    }

                    GameManager.Instance.SaveApplicationData();
                })
                .AddTo(disposable);
        }

        private void HandleConvertReward(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<ConcertRewardMessage>()
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

            var oldVal = playerData.Money;
            var newVal = SafetyAdd(oldVal, value, _settings.Player.MaxMoney);

            playerData.Money = newVal;
            MsgBroker.Instance.Publish(new MoneyChangedMessage {OldVal = oldVal, NewVal = newVal});
        }

        private void UpdateFans(int value)
        {
            var playerData = GameManager.Instance.PlayerData;

            var oldVal = playerData.Fans;
            var newVal = SafetyAdd(oldVal, value, _settings.Player.MaxFans);

            const int minFans = 0;
            newVal = Mathf.Max(newVal, minFans);

            playerData.Fans = newVal;
            MsgBroker.Instance.Publish(new FansChangedMessage {OldVal = oldVal, NewVal = newVal});
        }

        private static void UpdateExp(int value)
        {
            var playerData = GameManager.Instance.PlayerData;

            var oldVal = playerData.Exp;
            var newVal = playerData.Exp + value;

            const int minExp = 0;
            newVal = Mathf.Max(newVal, minExp);

            playerData.Exp = newVal;
            MsgBroker.Instance.Publish(new ExpChangedMessage {OldVal = oldVal, NewVal = newVal});
        }

        private static int SafetyAdd(int current, int increment, int maxValue)
        {
            return maxValue - current > increment
                ? current + increment
                : maxValue;
        }
    }
}