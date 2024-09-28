using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private void DoAlbum()
        {
            info.Cooldown = albumCooldown;

            // todo: do album
            var album = new AlbumInfo
            {
                Name = "ololo foo bar album"
            };

            // todo: apply album income

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_album_created",
                TextArgs = new[] { info.Name, album.Name },
                Sprite = info.Avatar,
                Popularity = RappersAPI.GetFansCount(info)
            });
        }
    }
}