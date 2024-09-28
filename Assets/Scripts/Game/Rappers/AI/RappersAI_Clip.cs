using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private void DoClip()
        {
            // todo: select track without clip OR return

            info.Cooldown = clipCooldown;

            // todo: do clip
            var clip = new ClipInfo
            {
                Name = "ololo foo bar clip"
            };

            // todo: apply clip income

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_clip_created",
                TextArgs = new[] { info.Name, clip.Name },
                Sprite = info.Avatar,
                Popularity = RappersAPI.GetFansCount(info)
            });
        }
    }
}