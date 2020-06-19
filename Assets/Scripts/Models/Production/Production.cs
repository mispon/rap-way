using System;
using Core;

namespace Models.Production
{
    /// <summary>
    /// Абстрактный класс продуктивной деятельности, позволяющий получить общую для всех проудктов информацию
    /// </summary>
    [Serializable]
    public abstract class Production
    {
        public int Id;
        public string Name;

        public int MoneyIncome;
        public int FansIncome;
        public int HypeIncome;

        public override string ToString()
            => $"{GameManager.Instance.PlayerData.Info.NickName} - {Name}";
    }
}