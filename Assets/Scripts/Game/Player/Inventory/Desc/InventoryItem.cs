using System;

namespace Game.Player.Inventory.Desc
{
    [Serializable]
    public class InventoryItem
    {
        public InventoryType Type;
        public string        Name;
        public bool          Equipped;
        public object        Raw;

        public T Value<T>()
        {
            return Raw is T val
                ? val
                : default;
        }

        public int ValueHash()
        {
            return Raw.GetHashCode();
        }
    }
}