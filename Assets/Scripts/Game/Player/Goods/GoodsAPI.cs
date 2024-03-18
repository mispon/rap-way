using System.Collections.Generic;
using System.Linq;
using Enums;
using Game.Player.State.Desc;

namespace Game.Player.Goods
{
    public class GoodsAPI
    {
        private static PlayerData data => GameManager.Instance.PlayerData;
        
        public float GetQualityImpact()
        {
            var equipTypes = new HashSet<GoodsType>
            {
                GoodsType.Micro,
                GoodsType.AudioCard,
                GoodsType.FxMixer,
                GoodsType.Acoustic
            };

            var playerEquipment = data.Goods
                .Where(g => equipTypes.Contains(g.Type))
                .GroupBy(g => g.Type)
                .ToDictionary(k => k, v => v.Max(g => g.QualityImpact));

            return playerEquipment.Sum(p => p.Value);
        }
    }
}