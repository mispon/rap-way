using Models.Info.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Базовый анализатор
    /// </summary>
    public abstract class Analyzer<T> : MonoBehaviour 
        //where T : Production //убрал, так как закинул сюда же SocialInfo, которое не наследуется от Production
    {
        /// <summary>
        /// Анализирует успешность деятельности игрока 
        /// </summary>
        public abstract void Analyze(T entity);
    }
}