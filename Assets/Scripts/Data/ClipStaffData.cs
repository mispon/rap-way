using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ClipStaff", menuName = "Data/Clip Staff")]
    public class ClipStaffData : ScriptableObject
    {
        [Header("Режиссёры")]
        public ClipStaff[] Directors;
        
        [Header("Операторы")]
        public ClipStaff[] Operators;
    }

    [Serializable]
    public class ClipStaff
    {
        public string NameKey;
        public int Skill;
        public int Salary;
    }
}