using System;
using System.Linq;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Наборы данных по каждому из типов товаров (для работы/понты)
    /// </summary>
    [CreateAssetMenu(fileName = "Goods", menuName = "Data/Goods")]
    public class GoodsData: SerializedScriptableObject
    {
        public GoodInfo[] Items;
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
        /// Иконки предмета
        /// </summary>
        public Sprite SquareImage;
        private Sprite RectImage;
        
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
        public Sprite PersonalPageImage => RectImage == null ? SquareImage : RectImage;
    }
}