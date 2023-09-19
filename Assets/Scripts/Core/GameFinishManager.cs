using Core.Interfaces;
using Core.Settings;
using Game;
using Game.Pages.GameFinish;
using UnityEngine;

namespace Core
{
    public class GameFinishManager : MonoBehaviour, IStarter
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private GameFinishPage page;
        
        public void OnStart()
        {
            PlayerManager.Instance.onFansAdd += OnFansChanged;
        }

        private void OnFansChanged(int value)
        {
            if (PlayerManager.Data.FinishPageShowed)
            {
                return;
            }
            
            if (value >= settings.MaxFans)
            {
                page.Open();
                PlayerManager.Data.FinishPageShowed = true;
            }
        }
    }
}