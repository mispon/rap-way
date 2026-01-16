using UnityEngine;

namespace _Project.Data.Settings
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "RapWay/Settings/TimeConfig")]
    public class TimeConfig : ScriptableObject
    {
        [Header("Balance")]
        [Tooltip("Сколько реальных секунд длится один игровой день")]
        [Min(0.1f)] public float RealSecondsPerDay = 10.0f;

        [Header("Calendar")]
        [Min(1)] public int DaysInWeek = 7;
        [Min(1)] public int DaysInMonth = 30;
        [Min(1)] public int DaysInYear = 360;
        
        [Header("Time Controls")]
        [Tooltip("Множитель скорости для нормальной игры")]
        public float NormalSpeed = 1.0f;
        
        [Tooltip("Множитель скорости для быстрой перемотки")]
        public float FastSpeed = 3.0f;
       
        [Tooltip("Множитель скорости для очень быстрой перемотки")]
        public float VeryFastSpeed = 6.0f;
    }
}