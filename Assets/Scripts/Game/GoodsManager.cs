using System;
using System.Linq;
using Enums;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Random = UnityEngine.Random;

namespace Game
{
    /// <summary>
    /// Менеджер генерации рабочих очков
    /// </summary>
    public class GoodsManager : Singleton<GoodsManager>
    {
        [SerializeField]
        private EquipmentItem[] equipments;

        /// <summary>
        /// Генерирует дополнительные очки работы в зависимости от наличия оборудования
        /// </summary>
        public int GenerateAdditionalWorkPoints()
        {
            var goods = PlayerManager.Data.Goods;
            var equipTypes = equipments.Select(e => e.Type).ToHashSet();

            int points = 0;
            foreach (var good in goods.Where(g => equipTypes.Contains(g.Type)))
            {
                var equip = GetEquipmentInfo(good.Type, good.Level);

                if (Random.Range(0f, 1f) <= equip.Chance)
                {
                    points += equip.WorkPoints;
                }
            }

            return points;
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
        [Range(0f, 1f)] public float Chance;

        public override string ToString()
        {
            return Name;
        }
    }
}