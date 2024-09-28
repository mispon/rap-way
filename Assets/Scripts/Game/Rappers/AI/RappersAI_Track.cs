using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private void DoTrack()
        {
            info.Cooldown = trackCooldown;

            // todo: do track
            var track = new TrackInfo
            {
                Name = "ololo foo bar track"
            };

            // todo: apply track income

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_track_created",
                TextArgs = new[] { info.Name, track.Name },
                Sprite = info.Avatar,
                Popularity = RappersAPI.GetFansCount(info)
            });
        }
    }
}