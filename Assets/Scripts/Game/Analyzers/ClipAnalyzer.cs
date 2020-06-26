using Models.Production;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор клипа
    /// </summary>
    public class ClipAnalyzer : Analyzer<ClipInfo>
    {
        /// <summary>
        /// Анализирует успешность клипа
        /// </summary>
        public override void Analyze(ClipInfo clip)
        {
            clip.Views = Random.Range(0, 1000);
            clip.Likes = Random.Range(0, 100);
            clip.Dislikes = Random.Range(0, 100);
        }
    }
}