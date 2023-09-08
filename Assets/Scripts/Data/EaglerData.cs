using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [CreateAssetMenu(fileName = "EaglerData", menuName = "Data/Eagler")]
    public class EaglerData : ScriptableObject
    {
        public string[] Nicknames;
        public string PositivePostKey;
        public int PositivePostsCount;
        public string NegativePostKey;
        public int NegativePostsCount;
    }
}