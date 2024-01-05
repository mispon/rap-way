using System;
using System.Linq;
using Enums;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Наборы данных по каждому из типов товаров (для работы/понты)
    /// </summary>
    [CreateAssetMenu(fileName = "Goods", menuName = "Data/Goods")]
    public class GoodsData: ScriptableObject
    {
        [Header("Рабочее"), ArrayElementTitle("Type")]
        public GoodInfo[] WorkTools;
        
        [Header("Личное"), ArrayElementTitle("Type")]
        public GoodInfo[] Swag;
        
        public GoodInfo[] AllItems => WorkTools.Concat(Swag).ToArray();
    }

    /// <summary>
    /// Наборы данных для отрисовки в магазине по конкретному типу товара
    /// </summary>
    [Serializable]
    public class GoodInfo
    {
        public GoodsType Type;
        
        /// <summary>
        /// Элементы отображения в магазине
        /// </summary>
        [ArrayElementTitle("Level", baseHeader = "Level")]
        public GoodUI[] UI;

        public short MaxItemLevel => UI.Max(el => el.Level);
    }
    
    /// <summary>
    /// Набор данных для отрисовки
    /// </summary>
    [Serializable]
    public struct GoodUI
    {
        /// <summary>
        /// Уровень предмета
        /// </summary>
        public short Level;
        
        /// <summary>
        /// Иконка предмета
        /// </summary>
        public Sprite Image;

        /// <summary>
        /// Иконка предмета на персональной странице
        /// </summary>
        [SerializeField] 
        private Sprite personalPageImage;
        
        /// <summary>
        /// Цена предмета
        /// </summary>
        public int Price;

        /// <summary>
        /// Баф к хайпу
        /// </summary>
        public int Hype;

        /// <summary>
        /// Возвращает иконку предмета на персональной странице, если она определена.
        /// Если неопределена - то иконку магазина
        /// </summary>
        public Sprite PersonalPageImage => personalPageImage == null ? Image : personalPageImage;
    }
}