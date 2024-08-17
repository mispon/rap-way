using System.Linq;
using Game.Player.Goods.Desc;
using MessageBroker;
using MessageBroker.Interfaces;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using UniRx;

namespace Game.Player.Goods
{
    public class GoodsEventsHandler : IMessagesHandler
    {
        public void RegisterHandlers(CompositeDisposable disposable)
        {
            HandleAddNewGood(disposable);
            HandleGoodExistsRequest(disposable);
        }

        private void HandleAddNewGood(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<AddNewGoodMessage>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;

                    var good = new Good
                    {
                        Type = e.Type,
                        Level = e.Level,
                        Hype = e.Hype,
                        QualityImpact = e.QualityImpact
                    };
                    playerData.Goods.Add(good);

                    MsgBroker.Instance.Publish(new ChangeHypeMessage());
                })
                .AddTo(disposable);
        }

        private void HandleGoodExistsRequest(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<GoodExistsRequest>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;

                    bool exists = e.IsNoAds
                        ? GameManager.Instance.LoadNoAds()
                        : playerData.Goods.Any(g => g.Type == e.Type && g.Level == e.Level);

                    MsgBroker.Instance.Publish(new GoodExistsResponse { Status = exists });
                })
                .AddTo(disposable);
        }
    }
}