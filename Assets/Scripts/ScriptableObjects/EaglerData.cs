using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "EaglerData", menuName = "Data/Eagler")]
    public class EaglerData : ScriptableObject
    {
        public string[] Nicknames;
        public string[] Hashtags;
        public string PositivePostKey;
        public int PositivePostsCount;
        public string NegativePostKey;
        public int NegativePostsCount;
    }
}