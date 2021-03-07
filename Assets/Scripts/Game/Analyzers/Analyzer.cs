using Core.Settings;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Базовый анализатор
    /// </summary>
    public abstract class Analyzer<T> : MonoBehaviour
    {
        protected const float ONE_PERCENT = 0.01f;

        protected GameSettings settings;

        private void Start()
        {
            settings = GameManager.Instance.Settings;
        }

        /// <summary>
        /// Анализирует успешность деятельности игрока 
        /// </summary>
        public abstract void Analyze(T social);

        /// <summary>
        /// Возвращает отношение прослушиваний к общему числу фанатов
        /// </summary>
        protected float GetListenRatio(int listensAmount)
        {
            return 1f * listensAmount / GetFans();
        }

        /// <summary>
        /// Возвращает количество фанатов
        /// </summary>
        protected int GetFans()
        {
            return Mathf.Max(PlayerManager.Data.Fans, settings.BaseFans);
        }
    }
}