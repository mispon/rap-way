using Utils;

namespace Game.UI.GameScreen
{
    /// <summary>
    /// Управление канвасом
    /// </summary>
    public class CanvasController: Singleton<CanvasController>
    {
        /// <summary>
        /// Изменяет видимость всех UI элементов
        /// </summary>
        public static void SetActive(bool value)
        {
            Instance.gameObject.SetActive(value);
        }
    }
}