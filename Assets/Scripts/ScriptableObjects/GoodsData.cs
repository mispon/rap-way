using System;
using Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Goods", menuName = "Data/Goods")]
    public class GoodsData: ScriptableObject
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
        
        public Sprite PersonalPageImage => RectImage == null ? SquareImage : RectImage;
    }

    [Serializable]
    public class SwagGood : GoodInfo
    {
        public int Hype;
    }
    
    [Serializable]
    public class DonateSwagGood : GoodInfo
    {
        public int Hype;
    }

    [Serializable]
    public class EquipGood : GoodInfo
    {
        [Range(0.0f, 0.21f)] public float QualityImpact;
    }
    
    [Serializable]
    public class DonateEquipGood : GoodInfo
    {
        [Range(0.0f, 0.41f)] public float QualityImpact;
    }

    [Serializable]
    public class DonateCoins : GoodInfo
    {
        public string ProductId;
        public int Amount;
    }
    
    [Serializable]
    public class NoAds : GoodInfo
    {
        public string ProductId;
    }
}