using System;
using Enums;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName ="SocialHypeImpact", menuName = "Data/Social Hype Impact")]
    public class SocialHypeImpactData: ScriptableObject
    {
        [Header("Базовые коэффициенты увеличения хайпа для каждого соц. действия")]
        [ArrayElementTitle("Type")]
        public SocialHypeMultiplier[] hypeMultipliers;
    }
    
    [Serializable]
    public struct SocialHypeMultiplier
    {
        public SocialType Type;
        public float Value;
    }
}