using System;

namespace Models.Eagler
{
    [Serializable]
    public class Eagle
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