using System;
using Enums;
using UnityEngine;

namespace Game.SocialNetworks.Email
{
    [Serializable]
    public class Email
    {
        public EmailsType Type;
        public string Title;
        public string[] TitleArgs;
        public string Content;
        public string[] ContentArgs;
        public string Sender;
        public string Date;
        public string SpriteName;
        public bool IsNew;

        [NonSerialized] public Action MainAction;
        [NonSerialized] public Action QuickAction;
        [NonSerialized] public Sprite Sprite;
    }
}