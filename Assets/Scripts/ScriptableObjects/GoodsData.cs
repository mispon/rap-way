using System;
using Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Goods", menuName = "Data/Goods")]
    public class GoodsData : ScriptableObject
    {
        [Header("Categories")]
        public CategoryInfo[] Categories;

        [Header("Goods Items")]
        public GoodsGroup[] Goods;
    }

    [Serializable]
    public class CategoryInfo
    {
        public GoodsType Type;
        public Sprite Icon;
    }

    [Serializable]
    public class GoodsGroup
    {
        public GoodsType Type;
        public GoodInfo[] Items;
    }

    [Serializable]
    public class GoodInfo
    {
        public string Name;
        public string Desc;
        public GoodsType Type;
        public short Level;
        public int Price;
        public Sprite SquareImage;
        public Sprite RectImage;
        public bool IsDonate;

        public Sprite PersonalPageImage => RectImage == null ? SquareImage : RectImage;

        // one_of:
        [Range(0.0f, 0.21f)]
        public float QualityImpact; // for equip items

        [Range(0, 21)]
        public int Hype; // for swag items
    }
}