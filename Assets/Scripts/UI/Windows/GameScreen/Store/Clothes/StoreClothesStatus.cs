using System;
using System.Collections.Generic;
using System.Linq;
using CharacterCreator2D;
using Core;
using Core.PropertyAttributes;
using Game.Player.Character;
using MessageBroker;
using MessageBroker.Messages.Store;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using TMPro;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using ClothingItem = Game.Player.Inventory.Desc.ClothingItem;
using ClothingItemInfo = ScriptableObjects.ClothingItem;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Store.Clothes
{
    [Serializable]
    public struct ClothesPriceSetting
    {
        public SlotCategory Slot;
        public int          Price;
    }

    public class StoreClothesStatus : MonoBehaviour
    {
        [SerializeField] private GameObject ownStatus;
        [SerializeField] private GameObject priceStatus;
        [SerializeField] private Sprite     clothingImage;
        [Space]
        [SerializeField] private TextMeshProUGUI priceLabel;
        [Space]
        [SerializeField] private Button buyButton;
        [SerializeField] private Button equipButton;
        [Space]
        [ArrayElementTitle(new[] {"Slot"})]
        [SerializeField] private ClothesPriceSetting[] prices;

        private SlotCategory _slot;

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<ClothesSlotChangedMessage>()
                .Subscribe(m => HandleClothesChanged(m.Slot, m.Index))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<ClothesSlotColorChangedMessage>()
                .Subscribe(m => HandleClothesChanged(m.Slot))
                .AddTo(_disposable);

            buyButton.onClick.AddListener(BuyClothingItem);
            equipButton.onClick.AddListener(EquipClothingItem);
        }

        private void HandleClothesChanged(SlotCategory slot, int index = 1)
        {
            if (index == 0)
            {
                HideAll();
                return;
            }

            _slot = slot;

            var clothingItem = GetClothingItem();
            if (string.IsNullOrEmpty(clothingItem.Name))
            {
                return;
            }

            var inventoryItem = PlayerAPI.Inventory.FindClothingItem(clothingItem);

            if (inventoryItem == null)
            {
                ToggleStatus(false, false);
                priceLabel.text = prices
                    .First(e => e.Slot == _slot)
                    .Price.ToString();
            } else
            {
                ToggleStatus(true, inventoryItem.Equipped);
            }
        }

        private void BuyClothingItem()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            const int clothesCategoryIndex = 4;

            var clothingItem = GetClothingItem();
            var price        = prices.First(e => e.Slot == _slot).Price;

            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.Shop_ItemCard,
                Context = new Dictionary<string, object>
                {
                    ["clothing_item_info"] = new ClothingItemInfo
                    {
                        Name        = clothingItem.Name,
                        Price       = price,
                        SquareImage = clothingImage,
                        Value       = clothingItem
                    },
                    ["category"] = clothesCategoryIndex
                }
            });
        }

        private void EquipClothingItem()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            var clothingItem = GetClothingItem();
            PlayerAPI.Inventory.EquipClothingItem(clothingItem);

            ToggleStatus(true, true);
        }

        private ClothingItem GetClothingItem()
        {
            var character = Character.Instance.Viewer;

            return new ClothingItem
            {
                Slot   = _slot,
                Name   = character.GetAssignedPart(_slot)?.name,
                Color1 = character.GetPartColor(_slot, ColorCode.Color1),
                Color2 = character.GetPartColor(_slot, ColorCode.Color2),
                Color3 = character.GetPartColor(_slot, ColorCode.Color3)
            };
        }

        private void HideAll()
        {
            ownStatus.SetActive(false);
            priceStatus.SetActive(false);
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
        }

        private void ToggleStatus(bool own, bool equipped)
        {
            ownStatus.SetActive(own);
            priceStatus.SetActive(!own);

            equipButton.gameObject.SetActive(own && !equipped);
            buyButton.gameObject.SetActive(!own);
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}