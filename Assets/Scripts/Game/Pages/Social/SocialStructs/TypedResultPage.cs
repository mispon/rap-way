using System;
using Enums;

namespace Game.Pages.Social.SocialStructs
{
    [Serializable]
    public struct TypedResultPage
    {
        public SocialType Type;
        public SocialResultPage Page;
    }
}