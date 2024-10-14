using System;
using System.Linq;
using Game.Production.Analyzers;
using Game.Rappers.Desc;
using Game.Settings;
using Game.SocialNetworks.Eagler;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoClip(RapperInfo rapper, GameSettings settings)
        {
            var track = SelectFreeTrack(rapper);
            if (track == null)
            {
                return;
            }

            Debug.Log($"[RAPPER AI] {rapper.Name} do clip");

            track.HasClip   = true;
            rapper.Cooldown = settings.Rappers.ClipCooldown;

            var clip = new ClipInfo
            {
                CreatorId      = rapper.Id,
                TrackId        = track.Id,
                Name           = track.Name,
                DirectorPoints = GenWorkPoints(rapper.Management, settings.Clip.WorkDuration),
                OperatorPoints = GenWorkPoints(rapper.Management, settings.Clip.WorkDuration)
            };

            ClipAnalyzer.Analyze(clip, settings);

            rapper.Fans = Math.Max(MIN_FANS_COUNT, rapper.Fans + clip.FansIncome);
            rapper.History.ClipList.Add(clip);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_clip_created",
                TextArgs   = new[] {rapper.Name, clip.Name},
                Sprite     = rapper.Avatar,
                Popularity = rapper.Fans
            });

            EaglerManager.Instance.GenerateEagles(1, rapper.Name, rapper.Fans, clip.Quality);
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