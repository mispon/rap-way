using Game.Player.State.Desc;

namespace Game.Player.State
{
    /// <summary>
    /// Player's state API
    /// </summary>
    public class StateAPI
    {
        private static PlayerData data => GameManager.Instance.PlayerData;
        
        public int GetFans()  => data.Fans;
        public int GetMoney() => data.Money;
        public int GetExp()   => data.Exp;
    }
}