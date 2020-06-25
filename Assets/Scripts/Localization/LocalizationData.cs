namespace Localization {
    /// <summary>
    /// Данные локализации
    /// </summary>
    [System.Serializable]
    public class LocalizationData 
    {
        public LocalizationItem[] items;
    }

    /// <summary>
    /// Единичный элемент локализации
    /// </summary>
    [System.Serializable]
    public class LocalizationItem 
    {
        public string key;
        public string value;
    }
}