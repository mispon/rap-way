using System;
using Core.PropertyAttributes;
using Game.Player.Inventory.Desc;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Store", menuName = "Data/Store")]
    public class StoreData : ScriptableObject
    {
        [Header("Categories List")]
        [ArrayElementTitle(new[] {"Type"})]
        public CategoryRow[] Categories;

        [Header("Categories Items")]
        [ArrayElementTitle(new[] {"Type"})]
        public CategoryItemsGroup[] Groups;
    }

    [Serializable]
    public class CategoryRow
    {
        public InventoryType Type;
        public Sprite        Icon;
    }

    [Serializable]
    public class CategoryItemsGroup
    {
        public InventoryType Type;
        public StoreItem[]   Items;
    }

    [Serializable]
    public class StoreItem
    {
        public string          Name;
        public string          Desc;
        public InventoryType   Type;
        public int             Price;
        public bool            IsDonate;
        public StoreItemValues Values;
        public Sprite          SquareImage;
        public Sprite          RectImage;
        public Sprite          PersonalPageImage => RectImage == null ? SquareImage : RectImage;
    }

    [Serializable]
    public class StoreItemValues
    {
        public int   Hype;
        public float Quality;
        public int   Level;
    }

    public class ClothingItem
    {
        public Sprite SquareImage;
        public string Name;
        public int    Price;
        public object Value;
    }
}