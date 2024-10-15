using System;
using System.Linq;
using Core.Analytics;
using Enums;
using Game.Production.Analyzers;
using Game.Rappers.Desc;
using Game.Settings;
using Game.SocialNetworks.Eagler;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoConcert(RapperInfo rapper, GameSettings settings, ConcertPlacesData concertData)
        {
            var album = SelectFreeAlbum(rapper);
            if (album == null)
            {
                return;
            }

            Debug.Log($"[RAPPER AI] {rapper.Name} do concert");
            AnalyticsManager.LogEvent(FirebaseGameEvents.RapperAI_CreateConcert);

            rapper.Cooldown = settings.Rappers.ConcertCooldown;
            album.ConcertAmounts++;

            var locationId = GetRandomLocationId(concertData);
            var location   = concertData.Places[locationId];

            var concert = new ConcertInfo
            {
                CreatorId        = rapper.Id,
                AlbumId          = album.Id,
                LocationId       = locationId,
                LocationName     = location.NameKey,
                LocationCapacity = location.Capacity,
                ManagementPoints = GenWorkPoints(rapper.Management, settings.Concert.WorkDuration),
                MarketingPoints  = GenWorkPoints(rapper.Management, settings.Concert.WorkDuration)
            };

            ConcertAnalyzer.Analyze(concert, settings);

            rapper.Fans = Math.Max(MIN_FANS_COUNT, rapper.Fans + concert.FansIncome);
            rapper.History.ConcertList.Add(concert);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_concert_finished",
                TextArgs   = new[] {rapper.Name, concert.LocationName},
                Sprite     = rapper.Avatar,
                Popularity = rapper.Fans
            });

            EaglerManager.Instance.GenerateEagles(1, rapper.Name, rapper.Fans, concert.Quality);
        }

        private static AlbumInfo SelectFreeAlbum(RapperInfo rapperInfo)
        {
            var albums = rapperInfo.History.AlbumList
                .Where(e => e.ConcertAmounts < 3)
                .ToArray();

            return albums.Length > 0
                ? albums[Random.Range(0, albums.Length)]
                : null;
        }

        private static int GetRandomLocationId(ConcertPlacesData concertData)
        {
            return Random.Range(0, concertData.Places.Length);
        }
    }
}