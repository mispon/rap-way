using System;
using CharacterCreator2D;
using UnityEngine;

namespace Game.Player.Inventory.Desc
{
    [Serializable]
    public struct ClothingItem
    {
        public SlotCategory Slot;
        public string       Name;
        public Color        Color1;
        public Color        Color2;
        public Color        Color3;

        public override int GetHashCode()
        {
            return HashCode.Combine(Slot, Name, Color1, Color2, Color3);
        }
    }
}