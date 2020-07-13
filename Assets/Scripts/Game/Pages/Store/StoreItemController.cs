using System;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Store
{
    /// <summary>
    /// Класс управkения UI-элементами конкретного товара
    /// </summary>
    public class StoreItemController : MonoBehaviour
    {
        [SerializeField] private Image iconImg;
        [SerializeField] private Text priceTxt;
        [SerializeField] private Button buyBtn;
        
        public GoodsType Type { get; private set; }
        public short Level { get; private set; }
        
        /// <summary>
        /// Инициализация UI-элементов
        /// </summary>
        public void Initialize(GoodsType type, GoodUI uiData, Action<GoodsType, short, int> onClickAction)
        {
            Type = type;
            Level = uiData.Level;
            
            iconImg.sprite = uiData.Image;
            priceTxt.text = $"{type} {uiData.Price}$";
            
            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(() => onClickAction(Type, Level, uiData.Price));
        }
    }
}