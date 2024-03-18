using System.Linq;
using Core.OrderedStarter;
using Game.Player.Goods.Desc;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using UniRx;
using UnityEngine;

namespace Game.Player.Goods
{
    public class GoodsEventsHandler : MonoBehaviour, IStarter
    {
        private readonly CompositeDisposable _disposable = new(); 
        
        public void OnStart()
        {
            HandleAddNewGood();
            HandleGoodExistsRequest();
        }

        private void HandleAddNewGood()
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
                .AddTo(_disposable);
        }

        private void HandleGoodExistsRequest()
        {
            MsgBroker.Instance
                .Receive<GoodExistsRequest>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    bool exists = e.IsNoAds
                        ? GameManager.Instance.LoadNoAds()
                        : playerData.Goods.Any(g => g.Type == e.Type && g.Level == e.Level);
                    
                    MsgBroker.Instance.Publish(new GoodExistsResponse {Status = exists});
                })
                .AddTo(_disposable);
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}