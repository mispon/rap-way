using System;
using System.Linq;
using CharacterCreator2D;
using Core.PropertyAttributes;
using Game.Player.Character;
using Game.Player.Inventory.Desc;
using MessageBroker;
using MessageBroker.Messages.Store;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
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
                .Receive<ClothesSlotChanged>()
                .Subscribe(HandleClothesChanged)
                .AddTo(_disposable);

            buyButton.onClick.AddListener(BuyClothingItem);
            equipButton.onClick.AddListener(EquipClothingItem);
        }

        private void HandleClothesChanged(ClothesSlotChanged m)
        {
            _slot = m.Slot;

            var clothingItem = GetClothingItem();
            if (string.IsNullOrEmpty(clothingItem.Name))
            {
                return;
            }

            var inventoryItem = PlayerAPI.Inventory.FindClothingItem(clothingItem);

            if (inventoryItem == null)
            {
                ownStatus.SetActive(false);
                priceStatus.SetActive(true);
                priceLabel.text = prices
                    .First(e => e.Slot == _slot)
                    .Price.ToString();
            } else
            {
                ownStatus.SetActive(true);
                priceStatus.SetActive(false);

                equipButton.interactable = !inventoryItem.Equipped;
            }
        }

        private void BuyClothingItem()
        {
            var clothingItem = GetClothingItem();
            var price        = prices.First(e => e.Slot == _slot).Price;
            // todo: show purchase window
        }

        private void EquipClothingItem()
        {
            var clothingItem = GetClothingItem();
            PlayerAPI.Inventory.EquipClothingItem(clothingItem);
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

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}