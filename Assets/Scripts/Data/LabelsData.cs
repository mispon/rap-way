using System;
using Models.Game;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Labels", menuName = "Data/Labels")]
    public class LabelsData : ScriptableObject
    {
        [ArrayElementTitle(new []{"Name"})]
        public LabelInfo[] Labels;
    }

    [Serializable]
    public class LabelInfo
    {
        public string Name;
        public Sprite Logo;
        public ExpValue Prestige;
        public ExpValue Production;
        public int Score;
        public bool IsCustom;
        // is custom player's label or not
        public bool IsPlayer;
        // is player's label frozen for non-payment or not
        public bool IsFrozen;
    }
}