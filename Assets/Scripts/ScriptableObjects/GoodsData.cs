using System;
using System.Collections.Generic;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Goods", menuName = "Data/Goods")]
    public class GoodsData: SerializedScriptableObject
    {
        [TabGroup("Categories")] 
        public CategoryInfo[] Categories;
        
        [TabGroup("Items")]
        public Dictionary<GoodsType, GoodInfo[]> Items;
    }

    [Serializable]
    public class CategoryInfo
    {
        public GoodsType Type;
        public Sprite Icon;
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
        [PropertyRange(0.0, 0.21)] public float QualityImpact;
    }
    
    [Serializable]
    public class DonateEquipGood : GoodInfo
    {
        [PropertyRange(0.0, 0.41)] public float QualityImpact;
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