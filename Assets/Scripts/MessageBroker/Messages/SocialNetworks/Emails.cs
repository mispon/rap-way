using System;

namespace MessageBroker.Messages.SocialNetworks
{
    public struct EmailMessage
    {
        public string Title;
        public string Content;
        public string Sender;
        public string SpriteName;
        public Action mainAction;
        public Action quickAction;
    }

    public struct ReadEmailMessage { }
}