using System.Collections.Generic;
using System.Linq;
using Core.Context;
using Game.Player.Inventory.Desc;
using Game.Player.State.Desc;

namespace Game.Player.Inventory
{
    public class InventoryAPI
    {
        private static PlayerData data => GameManager.Instance.PlayerData;

        public float GetQualityImpact()
        {
            var qualityTypes = new HashSet<InventoryType>
            {
                InventoryType.Micro,
                InventoryType.AudioCard,
                InventoryType.FxMixer,
                InventoryType.Acoustic
            };

            var equips = data.Inventory
                .Where(g => qualityTypes.Contains(g.Type))
                .GroupBy(g => g.Type)
                .ToDictionary(
                    k => k.Key, 
                    v => v.Max(g => g.Value<ValuesItem>().Quality)
                );
            
            return equips.Sum(p => p.Value);
        }

        public InventoryItem FindClothingItem(ClothingItem target)
        {
            var clothes = data.Inventory.Where(e => e.Type == InventoryType.Clothes);

            foreach (var item in clothes)
            {
                var value = item.Value<ClothingItem>();

                if (target.GetHashCode() == value.GetHashCode())
                {
                    return item;
                }
            }

            return null;
        }

        public void EquipClothingItem(ClothingItem target)
        {
            var clothes = data.Inventory.Where(e => e.Type == InventoryType.Clothes);

            foreach (var item in clothes)
            {
                var value = item.Value<ClothingItem>();

                if (value.Slot != target.Slot)
                {
                    continue;
                }

                // equip target clothing item and unequip others in that slot
                item.Equipped = target.GetHashCode() == value.GetHashCode();
            }
        }

        public List<ClothingItem> GetEquippedClothes()
        {
            return data.Inventory
                .Where(e => e.Type == InventoryType.Clothes)
                .Select(e => e.Value<ClothingItem>())
                .ToList();
        }
    }
}