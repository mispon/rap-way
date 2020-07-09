using Enums;
using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName ="Social", menuName = "Data/Social")]
    public class SocialData : ScriptableObject
    {
        [Header("Социальные действия")]
        [ArrayElementTitle("Type")] public Social[] Socials;

        [Header("Минимальная доля от состояния на благотворительность")]
        [SerializeField, Range(0.01f, 0.5f)]
        public float minCharityPercentage;
    }

    [Serializable]
    public class Social
    {
        public SocialType Type;
        public string WorkingPageHeader;
        [ElementTitle("Длительность (дн.)")] public int Duration;
        [ElementTitle("Восстановление (дн.)")] public int Cooldown;
    }
}
