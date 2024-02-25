using System;
using Core.PropertyAttributes;
using Game.Labels.Desc;
using Models.Game;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Labels", menuName = "Data/Labels")]
    public class LabelsData : ScriptableObject
    {
        [ArrayElementTitle(new []{"Name"})]
        public LabelInfo[] Labels;
    }
}