using System;
using Enums;
using UnityEngine;

namespace MessageBroker.Messages.SocialNetworks
{
    public struct EmailMessage
    {
        public EmailsType Type;
        public string Title;
        public string[] TitleArgs;
        public string Content;
        public string[] ContentArgs;
        public string Sender;
        public Sprite Sprite;
        public Action mainAction;
        public Action quickAction;
    }

    public struct ReadEmailMessage { }
}