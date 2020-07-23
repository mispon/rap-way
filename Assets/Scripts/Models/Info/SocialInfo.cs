using Data;
using Models.Player;
using UnityEngine.UI;

namespace Models.Info
{
    /// <summary>
    /// Информация по выполнению социального действия
    /// </summary>
    public class SocialInfo
    {
        public SocialActivity Activity;
        public Social Data;

        public string ExternalText;
        
        public int PlayerPoints;
        public int PrManPoints;

        public int CharityMoney;
        public int HypeIncome;
    }
}