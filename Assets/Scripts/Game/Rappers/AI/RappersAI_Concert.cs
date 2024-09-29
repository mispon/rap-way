using System;
using System.Linq;
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
        private void DoConcert()
        {
            var album = SelectFreeAlbum();
            if (album == null)
            {
                return;
            }
            album.ConcertAmounts++;

            info.Cooldown = concertCooldown;

            var locationId = GetRandomLocationId();
            var location = concertData.Places[locationId];

            var concert = new ConcertInfo
            {
                CreatorId = info.Id,
                AlbumId = album.Id,
                LocationId = locationId,
                LocationName = location.NameKey,
                LocationCapacity = location.Capacity,
                ManagementPoints = GenWorkPoints(info.Management, settings.Concert.WorkDuration),
                MarketingPoints = GenWorkPoints(info.Management, settings.Concert.WorkDuration),
            };

            concertAnalyzer.Analyze(concert);

            info.Fans = Math.Max(100, info.Fans + concert.FansIncome);
            info.History.ConcertList.Add(concert);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_concert_finished",
                TextArgs = new[] { info.Name, concert.LocationName },
                Sprite = info.Avatar,
                Popularity = info.Fans
            });

            EaglerManager.Instance.GenerateEagles(1, info.Name, info.Fans, concert.Quality);
        }

        private AlbumInfo SelectFreeAlbum()
        {
            var albums = info.History.AlbumList
                .Where(e => e.ConcertAmounts < 3)
                .ToArray();

            return albums.Length > 0
                ? albums[Random.Range(0, albums.Length)]
                : null;
        }

        private int GetRandomLocationId()
        {
            return Random.Range(0, concertData.Places.Length);
        }
    }
}