using System;

namespace RapWay.Application.Session
{
    public sealed class GameSessionLaunchRequest : IGameSessionLaunchRequest
    {
        private readonly object _sync = new();
        private GameSessionLaunchMode _requestedMode = GameSessionLaunchMode.MainMenu;
        private GameSessionLaunchMode _lastRequestedMode = GameSessionLaunchMode.MainMenu;

        public GameSessionLaunchMode LastRequestedMode
        {
            get
            {
                lock (_sync)
                {
                    return _lastRequestedMode;
                }
            }
        }

        public void Request(GameSessionLaunchMode mode)
        {
            lock (_sync)
            {
                _requestedMode = mode;
                _lastRequestedMode = mode;
            }
        }

        public GameSessionLaunchMode Consume()
        {
            lock (_sync)
            {
                GameSessionLaunchMode consumedMode = _requestedMode;
                _requestedMode = GameSessionLaunchMode.MainMenu;
                return consumedMode;
            }
        }
    }
}
