using System;
using Core.OrderedStarter;
using Game.Player;
using MessageBroker;
using MessageBroker.Messages.State;
using ScriptableObjects;
using UI.Windows.Pages.GameFinish;
using UniRx;
using UnityEngine;

namespace Game.Time
{
    public class GameFinishManager : MonoBehaviour, IStarter
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private GameFinishPage page;
        
        private IDisposable _disposable;
        
        public void OnStart()
        {
            _disposable = MainMessageBroker.Instance
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