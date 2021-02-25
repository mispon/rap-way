using Core.Settings;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Базовый анализатор
    /// </summary>
    public abstract class Analyzer<T> : MonoBehaviour
    {
        protected GameSettings settings;

        private void Start()
        {
            settings = GameManager.Instance.Settings;
        }

        /// <summary>
        /// Анализирует успешность деятельности игрока 
        /// </summary>
        public abstract void Analyze(T entity);
    }
}