using Models.Production;
using UnityEngine;

namespace Analyzers
{
    /// <summary>
    /// Базовый анализатор
    /// </summary>
    public abstract class Analyzer<T> : MonoBehaviour 
        where T : Production 
    {
        /// <summary>
        /// Анализирует успешность деятельности игрока 
        /// </summary>
        public abstract void Analyze(T entity);
    }
}