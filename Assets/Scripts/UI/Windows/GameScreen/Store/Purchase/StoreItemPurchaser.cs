using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Game;
using MessageBroker;
using MessageBroker.Messages.Player;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace UI.Windows.GameScreen.Store.Purchase
{
    public enum StoreItemType
    {
        // purchase for the game currency
        Game,
        // purchase for the donate currency
        Donate,
        // purchase donate currency for real money
        Purchase
    }
    
    public class StoreItemPurchaser : MonoBehaviour, IDetailedStoreListener
    {
        [SerializeField] private GoodsData data;
        
        private IStoreController _controller;

        private readonly Dictionary<string, DonateCoins> _coinItemsMap = new();
        private NoAds _noAdsItem;
        
        private IEnumerator Start()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            var storeItems = data.Items[GoodsType.Donate];
            foreach (var storeItem in storeItems)
            {
                switch (storeItem)
                {
                    case DonateCoins dc:
                        _coinItemsMap.Add(dc.ProductId, dc);
                        builder.AddProduct(dc.ProductId, ProductType.Consumable);
                        break;
                    case NoAds na:
                        _noAdsItem = na;
                        builder.AddProduct(na.ProductId, ProductType.Consumable);
                        break;
                    default:
                        continue;
                }
            }

            yield return new WaitUntil(() => GameManager.Instance.IsReady);
            UnityPurchasing.Initialize(this, builder);
        }

        public static StoreItemType GetStoreItemType(GoodInfo item)
        {
            return item switch
            {
                SwagGood => StoreItemType.Game,
                EquipGood => StoreItemType.Game,
                
                DonateSwagGood => StoreItemType.Donate,
                DonateEquipGood => StoreItemType.Donate,
                
                DonateCoins => StoreItemType.Purchase,
                NoAds => StoreItemType.Purchase,
                
                _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
            };
        }

        public static AddNewGoodMessage CreateNewGoodEvent(GoodInfo item)
        {
            var goodEvent = new AddNewGoodMessage
            {
                Type = item.Type,
                Level = item.Level,
            };

            switch (item)
            {
                case SwagGood sg:
                    goodEvent.Hype = sg.Hype;
                    break;
                case DonateSwagGood dsg:
                    goodEvent.Hype = dsg.Hype;
                    break;
                case EquipGood eg:
                    goodEvent.QualityImpact = eg.QualityImpact;
                    break;
                case DonateEquipGood deg:
                    goodEvent.QualityImpact = deg.QualityImpact;
                    break;
            }

            return goodEvent;
        }

        public void PurchaseStoreItem(GoodInfo item)
        {
            string productId = item switch
            {
                DonateCoins dc => dc.ProductId,
                NoAds na       => na.ProductId,
                _              => ""
            };

            if (string.IsNullOrEmpty(productId))
                return;
            
            _controller?.InitiatePurchase(productId);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var productId = purchaseEvent.purchasedProduct.definition.id;

            if (_coinItemsMap.TryGetValue(productId, out var item))
            {
                MsgBroker.Instance.Publish(new AddDonateMessage {Amount = item.Amount});
            } 
            else if (productId == _noAdsItem.ProductId)
            {
                MsgBroker.Instance.Publish(new NoAdsPurchaseMessage());
            } 
            
            return PurchaseProcessingResult.Complete;
        }
        
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            CheckNoAdsPurchase();
        }

        private void CheckNoAdsPurchase()
        {
            if (GameManager.Instance.LoadNoAds())
                // already exists
                return;
            
            var product = _controller.products.WithID(_noAdsItem.ProductId);
            if (product is {hasReceipt: true})
            {
                MsgBroker.Instance.Publish(new NoAdsPurchaseMessage());
            }
        }

        #region StoreCallbacks

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"{product.definition.id} purchase failed, reason: {failureReason}");
        }
        
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"{product.definition.id} purchase failed, reason: {failureDescription.reason}, message: {failureDescription.message}");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Failed to initialize store, reason: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Failed to initialize store, reason: {error}, message: {message}");
        }

        #endregion
    }
}