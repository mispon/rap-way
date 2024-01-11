using System;
using Core.Interfaces;
using Core.Settings;
using Game;
using Game.Pages.GameFinish;
using MessageBroker.Messages.State;
using UniRx;
using UnityEngine;

namespace Core
{
    public class GameFinishManager : MonoBehaviour, IStarter
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private GameFinishPage page;
        
        private IDisposable _disposable;
        
        public void OnStart()
        {
            _disposable = GameManager.Instance.MessageBroker
                .Receive<FansChangedEvent>()
                .Subscribe(e => OnFansChanged(e.NewVal));
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

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}