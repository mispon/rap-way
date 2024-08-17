using System;
using Core.Localization;
using Extensions;
using Game.Production;

namespace Models.Production
{
    /// <summary>
    /// Информация о выпущенном клипе
    /// </summary>
    [Serializable]
    public class ClipInfo : ProductionBase
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
            $"{Convert.ToInt32(Quality * 100)}%",
            Views.GetDisplay(),
            $"+{Likes.GetDisplay()} / -{Dislikes.GetDisplay()}"
        };

        public override string GetLog()
        {
            string trackName = ProductionManager.GetTrackName(TrackId);
            return $"{Timestamp}: {LocalizationManager.Instance.Get("log_clip")} {trackName}";
        }
    }
}