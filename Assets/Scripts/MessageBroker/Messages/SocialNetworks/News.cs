using UnityEngine;

namespace MessageBroker.Messages.SocialNetworks
{
    public struct NewsMessage
    {
        public string Text;
        public string[] TextArgs;
        public Sprite Sprite;
        public int Popularity;
    }
}