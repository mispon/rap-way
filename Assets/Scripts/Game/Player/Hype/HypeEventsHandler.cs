using System.Linq;
using MessageBroker;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.State;
using MessageBroker.Messages.Time;
using UniRx;
using UnityEngine;

namespace Game.Player.Hype
{
    public class HypeEventsHandler : BaseEventsHandler
    {
        protected override void RegisterHandlers()
        {
            HandleDayLeft();
            HandleChangeHype();
            HandleProductionReward();
        }

        private void HandleDayLeft()
        {
            MainMessageBroker.Instance
                .Receive<DayLeftEvent>()
                .Subscribe(e =>
                {
                    if (e.Day % 3 == 0)
                    {
                        MainMessageBroker.Instance.Publish(new ChangeHypeEvent {Amount = -1});
                    } 
                })
                .AddTo(disposable);
        }
        
        private void HandleChangeHype()
        {
            MainMessageBroker.Instance
                .Receive<ChangeHypeEvent>()
                .Subscribe(e => UpdateHype(e.Amount))
                .AddTo(disposable);
        }
        
        private void HandleProductionReward()
        {
            MainMessageBroker.Instance
                .Receive<ProductionRewardEvent>()
                .Subscribe(e => UpdateHype(e.HypeIncome))
                .AddTo(disposable);
        }
        
        private static void UpdateHype(int value)
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
    }
}