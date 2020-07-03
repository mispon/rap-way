using System;
using Enums;
using UnityEngine.UI;

namespace Game.Pages.Social.SocialStructs
{
    /// <summary>
    /// Элементы интерфейса социального действия
    /// </summary>
    [Serializable]
    public struct SocialSettingsUi
    {
        public SocialType Type;
        public Button Btn;
        public InputField ExternalTextField;
        public Slider ExternalSlider;
    }
}