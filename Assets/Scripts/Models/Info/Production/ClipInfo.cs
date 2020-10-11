using System;
using Core;

namespace Models.Info.Production
{
    /// <summary>
    /// Информация о выпущенном клипе
    /// </summary>
    [Serializable]
    public class ClipInfo: Production
    {
        public int TrackId;

        public int DirectorSkill;
        public int OperatorSkill;
        
        public int DirectorPoints;
        public int OperatorPoints;

        public int Views;
        public int Likes;
        public int Dislikes;

        public override string[] HistoryInfo => new[]
        {
            Name,
            Views.ToString(),
            $"{Likes} / {Dislikes}"
        };

        public override string GetLog() {
            string trackName = ProductionManager.GetTrackName(TrackId);
            return $"{Timestamp}: Завершил съемку клипа на трек {trackName}";
        }
    }
}