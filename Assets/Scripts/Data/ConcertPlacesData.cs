using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ConcertPlaces", menuName = "Data/Concert Places")]
    public class ConcertPlacesData : ScriptableObject
    {
        [Header("Концертные площадки")]
        public ConcertPlace[] Places;
    }

    [Serializable]
    public class ConcertPlace
    {
        public string NameKey;
        public int FansRequirement;
        public int Cost;
    }
}