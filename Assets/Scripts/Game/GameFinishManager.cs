using System;
using Core.OrderedStarter;
using Game.Player;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;
using UnityEngine;

namespace Game
{
    public class GameFinishManager : MonoBehaviour, IStarter
    {
        [SerializeField] private GameSettings settings;
        
        private IDisposable _disposable;
        
        public void OnStart()
        {
            _disposable = MsgBroker.Instance
                .Receive<FansChangedMessage>()
                .Subscribe(e => OnFansChanged(e.NewVal));
        }

        private void OnFansChanged(int value)
        {
            if (PlayerManager.Data.FinishPageShowed)
                return;
            
            
            if (value >= settings.Player.MaxFans)
            {
                MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.GameFinish));
                PlayerManager.Data.FinishPageShowed = true;
            }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}