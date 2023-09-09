using System;
using System.Linq;
using Enums;
using UnityEngine;
using Utils;

namespace Game
{
    /// <summary>
    /// Менеджер предметов игрока
    /// </summary>
    public class GoodsManager : Singleton<GoodsManager>
    {
        [SerializeField]
        private EquipmentItem[] equipments;

        /// <summary>
        /// Генерирует дополнительные очки работы в зависимости от наличия оборудования
        /// </summary>
        public float GetQualityImpact()
        {
            var goods = PlayerManager.Data.Goods;
            var equipTypes = equipments.Select(e => e.Type).ToHashSet();

            float impactTotal = 0;
            foreach (var good in goods.Where(g => equipTypes.Contains(g.Type)))
            {
                var equip = GetEquipmentInfo(good.Type, good.Level);
                impactTotal += equip.Impact;
            }

            return impactTotal;
        }

        /// <summary>
        /// Возвращает информацию с настройками оборудования
        /// </summary>
        public EquipmentItem GetEquipmentInfo(GoodsType type, int level)
        {
            return equipments.FirstOrDefault(e => e.Type == type && e.Level == level);
        }
    }

    [Serializable]
    public class EquipmentItem
    {
        public string Name;
        public GoodsType Type;
        public short Level;
        public int WorkPoints;
        [Range(0.0f, 0.2f)] public float Impact;

        public override string ToString()
        {
            return Name;
        }
    }
}