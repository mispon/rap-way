using System;
using Core.OrderedStarter;
using Game.Player;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using UI.Windows.Pages.GameFinish;
using UniRx;
using UnityEngine;

namespace Game
{
    public class GameFinishManager : MonoBehaviour, IStarter
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private GameFinishPage page;
        
        private IDisposable _disposable;
        
        public void OnStart()
        {
            _disposable = MainMessageBroker.Instance
                .Receive<FansChangedMessage>()
                .Subscribe(e => OnFansChanged(e.NewVal));
        }

        private void OnFansChanged(int value)
        {
            if (PlayerManager.Data.FinishPageShowed)
            {
                return;
            }
            
            if (value >= settings.Player.MaxFans)
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