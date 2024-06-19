using System;

namespace Game.Socials.Twitter
{
    [Serializable]
    public class Twit
    {
        public string Nickname;
        public string Date;
        public string Message;
        public int Likes;
        public int Views;
        public int Shares;
        public string Tags;
        public bool IsUser;
    }
}