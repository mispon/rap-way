using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Goods", menuName = "Data/Goods")]
    public class GoodsData: SerializedScriptableObject
    {
        [TabGroup("Categories")] 
        public Dictionary<GoodsType, Sprite> Categories;
        
        [TabGroup("Items")]
        public Dictionary<GoodsType, GoodInfo[]> Items;
    }
    
    [Serializable]
    public class GoodInfo
    {
        public short Level;
        public int Price;
        public int Hype;
        public Sprite SquareImage;
        public Sprite RectImage;
        
        public Sprite PersonalPageImage => RectImage == null ? SquareImage : RectImage;
    }
}