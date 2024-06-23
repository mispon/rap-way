using System;

namespace Game.SocialNetworks.Email
{
    [Serializable]
    public class Email
    {
        public string Title;
        public string Content;
        public string Sender;
        public string SpriteName;
        public string Date;
        public bool   IsNew;

        [NonSerialized] public Action MainAction;
        [NonSerialized] public Action QuickAction;
    }
}