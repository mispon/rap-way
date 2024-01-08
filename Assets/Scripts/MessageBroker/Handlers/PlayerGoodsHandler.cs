using System.Linq;
using Core.Interfaces;
using Game;
using MessageBroker.Messages.Goods;
using MessageBroker.Messages.State;
using Models.Player;
using UniRx;
using UnityEngine;

namespace MessageBroker.Handlers
{
    public class PlayerGoodsHandler : MonoBehaviour, IStarter
    {
        private IMessageBroker _messageBroker;
        
        private PlayerData _playerData;

        public void OnStart()
        {
            _messageBroker = GameManager.Instance.MessageBroker;
            _playerData = GameManager.Instance.PlayerData;

            HandleAddNewGood();
            HandleGoodExistsRequest();
        }

        private void HandleAddNewGood()
        {
            _messageBroker
                .Receive<AddNewGoodEvent>()
                .Subscribe(e =>
                {
                    var good = new Good
                    {
                        Type = e.Type,
                        Level = e.Level,
                        Hype = e.Hype,
                        QualityImpact = e.QualityImpact
                    };
                    
                    _playerData.Goods.Add(good);
                    _messageBroker.Publish(new ChangeHypeEvent());
                });
        }

        private void HandleGoodExistsRequest()
        {
            _messageBroker
                .Receive<GoodExistsRequest>()
                .Subscribe(e =>
                {
                    bool exists = e.IsNoAds
                        ? GameManager.Instance.LoadNoAds()
                        : _playerData.Goods.Any(g => g.Type == e.Type && g.Level == e.Level);
                    
                    _messageBroker.Publish(new GoodExistsResponse {Status = exists});
                });
        }
    }
}