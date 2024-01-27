using System.Collections.Generic;
using System.Linq;
using Core.OrderedStarter;
using Enums;
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
        private readonly CompositeDisposable _disposable = new(); 
        
        public void OnStart()
        {
            HandleAddNewGood();
            HandleGoodExistsRequest();
            HandleQualityImpactRequest();
        }

        private void HandleAddNewGood()
        {
            MainMessageBroker.Instance
                .Receive<AddNewGoodEvent>()
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
                    
                    MainMessageBroker.Instance.Publish(new ChangeHypeEvent());
                })
                .AddTo(_disposable);
        }

        private void HandleGoodExistsRequest()
        {
            MainMessageBroker.Instance
                .Receive<GoodExistsRequest>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    bool exists = e.IsNoAds
                        ? GameManager.Instance.LoadNoAds()
                        : playerData.Goods.Any(g => g.Type == e.Type && g.Level == e.Level);
                    
                    MainMessageBroker.Instance.Publish(new GoodExistsResponse {Status = exists});
                })
                .AddTo(_disposable);
        }

        private void HandleQualityImpactRequest()
        {
            MainMessageBroker.Instance
                .Receive<GoodsQualityImpactRequest>()
                .Subscribe(_ =>
                {
                    var playerData = GameManager.Instance.PlayerData;
                    
                    var equipTypes = new HashSet<GoodsType>
                    {
                        GoodsType.Micro,
                        GoodsType.AudioCard,
                        GoodsType.FxMixer,
                        GoodsType.Acoustic
                    };

                    var playerEquipment = playerData.Goods
                        .Where(g => equipTypes.Contains(g.Type))
                        .GroupBy(g => g.Type)
                        .ToDictionary(k => k, v => v.Max(g => g.QualityImpact));

                    float impactTotal = playerEquipment.Sum(p => p.Value);
                    MainMessageBroker.Instance.Publish(new GoodsQualityImpactResponse {Value = impactTotal});
                })
                .AddTo(_disposable);
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}