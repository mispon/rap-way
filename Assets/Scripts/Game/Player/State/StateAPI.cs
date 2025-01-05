using Game.Player.State.Desc;

namespace Game.Player.State
{
    /// <summary>
    ///     Player's state API
    /// </summary>
    public class StateAPI
    {
        private static PlayerData data => GameManager.Instance.PlayerData;

        public int GetFans()
        {
            return data.Fans;
        }

        public int GetMoney()
        {
            return data.Money;
        }

        public int GetExp()
        {
            return data.Exp;
        }

        public int GetLevel()
        {
            var level = -5;

            level += data.Stats.Vocobulary.Value;
            level += data.Stats.Bitmaking.Value;
            level += data.Stats.Flow.Value;
            level += data.Stats.Charisma.Value;
            level += data.Stats.Management.Value;
            level += data.Stats.Marketing.Value;

            return level;
        }
    }
}