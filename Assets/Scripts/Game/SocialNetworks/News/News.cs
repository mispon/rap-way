using System;
using UnityEngine;

namespace Game.SocialNetworks.News
{
    [Serializable]
    public class News
    {
        public string Text;
        public string Date;
        public string SpriteName;
        public int Popularity;

        [NonSerialized] public Sprite Sprite;
    }
}