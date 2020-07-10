namespace Game.Pages.Training.Tabs
{
    /// <summary>
    /// Вкладка изучения новых тематик и стилей
    /// </summary>
    public class TrainingToneTab : TrainingTab
    {
        /// <summary>
        /// Активирует / деактивирует вкладку
        /// </summary>
        public override void Toggle(bool isOpen)
        {
            if (isOpen)
            {
                // todo: reset controls
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}