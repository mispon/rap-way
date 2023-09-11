using Enums;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;

namespace Game.Pages.Store
{
    /// <summary>
    /// Страница с информацией о товаре
    /// </summary>
    public class StoreItemInfoPage : Page
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemDesc;
        [SerializeField] private Text itemPrice;
        [SerializeField] private Text itemHype;
        [SerializeField] private GameObject hypeContainer;

        /// <summary>
        /// Отображает описание
        /// </summary>
        public void Show(Sprite icon, GoodsType type, int level, int price, int hype)
        {
            var equip = GoodsManager.Instance.GetEquipmentInfo(type, level);

            itemImage.sprite = icon;
            itemName.text = $"{GetLocale(type.GetDescription())} #{level}".ToUpper();
            itemDesc.text = equip != null
                ? GetLocale("equip_desc", equip.Impact * 100, equip.WorkPoints)
                : GetLocale(GetStuffKey(type, level));
            
            itemPrice.text = price.GetMoney();

            if (hype > 0)
            {
                itemHype.text = $"+{hype}";
                hypeContainer.SetActive(true);
            } else
            {
                hypeContainer.SetActive(false);
            }

            Open();
        }

        /// <summary>
        /// Возвращает ключ локализацтт
        /// </summary>
        private static string GetStuffKey(GoodsType type, int level)
        {
            switch (type)
            {
                case GoodsType.Car:
                    return $"car_desc_{level}";
                case GoodsType.Swatches:
                    return $"clock_desc_{level}";
                case GoodsType.Chain:
                    return $"chain_desc_{level}";
                case GoodsType.Grillz:
                    return $"grillz_desc_{level}";
                default:
                    throw new RapWayException($"Key {type} not expected here!");
            }
        }
    }
}