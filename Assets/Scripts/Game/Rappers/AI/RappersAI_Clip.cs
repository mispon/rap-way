using System;
using System.Linq;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using Random = UnityEngine.Random;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private void DoClip()
        {
            var track = SelectFreeTrack();
            if (track == null)
            {
                return;
            }
            track.HasClip = true;

            info.Cooldown = clipCooldown;

            var clip = new ClipInfo
            {
                CreatorId = info.Id,
                TrackId = track.Id,
                Name = track.Name,
                DirectorPoints = GenWorkPoints(info.Management, settings.Clip.WorkDuration),
                OperatorPoints = GenWorkPoints(info.Management, settings.Clip.WorkDuration),
            };

            clipAnalyzer.Analyze(clip);

            info.Fans = Math.Max(100, info.Fans + clip.FansIncome);
            info.History.ClipList.Add(clip);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_clip_created",
                TextArgs = new[] { info.Name, clip.Name },
                Sprite = info.Avatar,
                Popularity = info.Fans
            });
        }

        private TrackInfo SelectFreeTrack()
        {
            var tracks = info.History.TrackList
                .Where(e => !e.HasClip)
                .ToArray();

            return tracks.Length > 0
                ? tracks[Random.Range(0, tracks.Length)]
                : null;
        }
    }
}