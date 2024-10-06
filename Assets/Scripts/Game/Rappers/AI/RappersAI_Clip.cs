using System;
using System.Linq;
using Game.Production.Analyzers;
using Game.Rappers.Desc;
using Game.Settings;
using Game.SocialNetworks.Eagler;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using Random = UnityEngine.Random;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoClip(RapperInfo rapperInfo, GameSettings settings)
        {
            var track = SelectFreeTrack(rapperInfo);
            if (track == null)
            {
                return;
            }

            track.HasClip       = true;
            rapperInfo.Cooldown = settings.Rappers.ClipCooldown;

            var clip = new ClipInfo
            {
                CreatorId      = rapperInfo.Id,
                TrackId        = track.Id,
                Name           = track.Name,
                DirectorPoints = GenWorkPoints(rapperInfo.Management, settings.Clip.WorkDuration),
                OperatorPoints = GenWorkPoints(rapperInfo.Management, settings.Clip.WorkDuration)
            };

            ClipAnalyzer.Analyze(clip, settings);

            rapperInfo.Fans = Math.Max(MIN_FANS_COUNT, rapperInfo.Fans + clip.FansIncome);
            rapperInfo.History.ClipList.Add(clip);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_clip_created",
                TextArgs   = new[] {rapperInfo.Name, clip.Name},
                Sprite     = rapperInfo.Avatar,
                Popularity = rapperInfo.Fans
            });

            EaglerManager.Instance.GenerateEagles(1, rapperInfo.Name, rapperInfo.Fans, clip.Quality);
        }

        private static TrackInfo SelectFreeTrack(RapperInfo rapperInfo)
        {
            var tracks = rapperInfo.History.TrackList
                .Where(e => !e.HasClip)
                .ToArray();

            return tracks.Length > 0
                ? tracks[Random.Range(0, tracks.Length)]
                : null;
        }
    }
}