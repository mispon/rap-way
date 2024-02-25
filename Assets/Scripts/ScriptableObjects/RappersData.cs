using System.Collections.Generic;
using Core.PropertyAttributes;
using Game.Rappers.Desc;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Наборы данных о реальных исполнителях
    /// </summary>
    [CreateAssetMenu(fileName = "Rappers", menuName = "Data/Rappers")]
    public class RappersData : ScriptableObject
    {
        [ArrayElementTitle(new []{"Name"})]
        public List<RapperInfo> Rappers;
    }
}