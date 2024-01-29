using System;

namespace Models.Production
{
    /// <summary>
    /// Абстрактный класс продуктивной деятельности, позволяющий получить общую для всех проудктов информацию
    /// </summary>
    [Serializable]
    public abstract class ProductionBase
    {
        public int Id;
        public string Name;
        public int MoneyIncome;
        public int FansIncome;
        public string Timestamp;
        public float Quality;
        public bool IsHit;

        /// <summary>
        /// Массив строк из полей класса для заполнения в таблице Истории
        /// </summary>
        public virtual string[] HistoryInfo => Array.Empty<string>();

        /// <summary>
        /// Возвращает информацию о событии 
        /// </summary>
        public abstract string GetLog();
    }
}