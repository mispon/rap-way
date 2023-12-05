using Core.Interfaces;
using UnityEngine;

namespace Core
{
    public class AdsManager : MonoBehaviour, IStarter
    {
        [SerializeField] private int adsFrequency = 6;
        
        public void OnStart()
        {
            TimeManager.Instance.onMonthLeft += OnMonthLeftHandler;
        }

        private void OnMonthLeftHandler()
        {
            if (TimeManager.Instance.Now.Month % adsFrequency == 0)
            {
            }
        }
    }
}