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
        [Obsolete]
        [Header("Для работы"), ArrayElementTitle("Type")]
        public GoodInfo[] WorkTools;

        [Obsolete]
        [Header("Понты"), ArrayElementTitle("Type")]
        public GoodInfo[] Swag;

        /// <summary>
        /// Возвращает наборы всех товаров 
        /// </summary>
        public GoodInfo[] AllItems => WorkTools.Concat(Swag).ToArray();
    }

    /// <summary>
    /// Наборы данных для отрисовки в магазине по конкретному типу товара
    /// </summary>
    [Serializable]
    public class GoodInfo
    {
        /// <summary>
        /// Тип предмета
        /// </summary>
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
        /// Цена предмета
        /// </summary>
        public int Price;
    }
}