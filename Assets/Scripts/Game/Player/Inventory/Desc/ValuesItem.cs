using System;

namespace Game.Player.Inventory.Desc
{
    [Serializable]
    public struct ValuesItem
    {
        public float Quality;
        public int   Hype;
        public int   Level;

        public override int GetHashCode()
        {
            return HashCode.Combine(Quality, Hype, Level);
        }
    }
}