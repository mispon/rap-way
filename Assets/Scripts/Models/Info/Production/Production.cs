using System;

namespace Models.Info.Production
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
        
        /// <summary>
        /// Массив строк из полей класса для заполнения в таблице Истории
        /// </summary>
        public virtual string[] HistoryInfo => new string[0];
    }
}