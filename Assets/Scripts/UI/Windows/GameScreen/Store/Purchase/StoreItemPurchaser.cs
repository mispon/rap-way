using Enums;
using MessageBroker.Messages.Player;
using ScriptableObjects;
using UnityEngine;

namespace UI.Windows.GameScreen.Store.Purchase
{
    public class StoreItemPurchaser : MonoBehaviour
    {
        public static AddNewGoodMessage CreateNewGoodEvent(GoodInfo item)
        {
            var goodEvent = new AddNewGoodMessage
            {
                Type = item.Type,
                Level = item.Level
            };

            switch (item.Type)
            {
                case GoodsType.Micro:
                case GoodsType.Acoustic:
                case GoodsType.AudioCard:
                case GoodsType.FxMixer:
                    goodEvent.QualityImpact = item.QualityImpact;
                    break;

                case GoodsType.Car:
                case GoodsType.Chain:
                case GoodsType.Swatches:
                case GoodsType.Grillz:
                    goodEvent.Hype = item.Hype;
                    break;
            }

            return goodEvent;
        }
    }
}