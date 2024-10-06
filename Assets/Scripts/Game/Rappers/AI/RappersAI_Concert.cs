using System;
using System.Linq;
using Game.Production.Analyzers;
using Game.Rappers.Desc;
using Game.Settings;
using Game.SocialNetworks.Eagler;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using ScriptableObjects;
using Random = UnityEngine.Random;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoConcert(RapperInfo rapperInfo, GameSettings settings, ConcertPlacesData concertData)
        {
            var album = SelectFreeAlbum(rapperInfo);
            if (album == null)
            {
                return;
            }

            album.ConcertAmounts++;
            rapperInfo.Cooldown = settings.Rappers.ConcertCooldown;

            var locationId = GetRandomLocationId(concertData);
            var location   = concertData.Places[locationId];

            var concert = new ConcertInfo
            {
                CreatorId        = rapperInfo.Id,
                AlbumId          = album.Id,
                LocationId       = locationId,
                LocationName     = location.NameKey,
                LocationCapacity = location.Capacity,
                ManagementPoints = GenWorkPoints(rapperInfo.Management, settings.Concert.WorkDuration),
                MarketingPoints  = GenWorkPoints(rapperInfo.Management, settings.Concert.WorkDuration)
            };

            ConcertAnalyzer.Analyze(concert, settings);

            rapperInfo.Fans = Math.Max(MIN_FANS_COUNT, rapperInfo.Fans + concert.FansIncome);
            rapperInfo.History.ConcertList.Add(concert);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_concert_finished",
                TextArgs   = new[] {rapperInfo.Name, concert.LocationName},
                Sprite     = rapperInfo.Avatar,
                Popularity = rapperInfo.Fans
            });

            EaglerManager.Instance.GenerateEagles(1, rapperInfo.Name, rapperInfo.Fans, concert.Quality);
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