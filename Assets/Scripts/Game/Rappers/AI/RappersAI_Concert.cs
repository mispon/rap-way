using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private void DoConcert()
        {
            info.Cooldown = concertCooldown;

            // todo: do concert
            var concert = new ConcertInfo
            {
                Name = "ololo foo bar concert"
            };

            // todo: apply concert income

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_concert_finished",
                TextArgs = new[] { info.Name, concert.Name },
                Sprite = info.Avatar,
                Popularity = RappersAPI.GetFansCount(info)
            });
        }
    }
}