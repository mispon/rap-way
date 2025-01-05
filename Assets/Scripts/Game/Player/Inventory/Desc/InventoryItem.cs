using System;
using UnityEngine;

namespace Game.Player.Inventory.Desc
{
    [Serializable]
    public class InventoryItem
    {
        public InventoryType Type;
        public string        Name;
        public bool          Equipped;
        public string        Raw;

        public T Value<T>()
        {
            return JsonUtility.FromJson<T>(Raw);
        }

        public void SetValue<T>(T value)
        {
            Raw = JsonUtility.ToJson(value);
        }

        public int ValueHash()
        {
            return Raw.GetHashCode();
        }
    }
}