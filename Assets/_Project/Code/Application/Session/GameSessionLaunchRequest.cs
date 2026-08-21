using System;

namespace RapWay.Application.Session
{
    public sealed class GameSessionLaunchRequest : IGameSessionLaunchRequest
    {
        private readonly object _sync = new();
        private GameSessionLaunchRequestData _requested = new(GameSessionLaunchMode.MainMenu, null);
        private GameSessionLaunchRequestData _lastRequested = new(GameSessionLaunchMode.MainMenu, null);

        public GameSessionLaunchMode LastRequestedMode
        {
            get
            {
                lock (_sync)
                {
                    return _lastRequested.Mode;
                }
            }
        }

        public CareerStartTemplateId? LastRequestedStartTemplateId
        {
            get
            {
                lock (_sync)
                {
                    return _lastRequested.StartTemplateId;
                }
            }
        }

        public void Request(GameSessionLaunchMode mode, CareerStartTemplateId? startTemplateId = null)
        {
            lock (_sync)
            {
                GameSessionLaunchRequestData request = new(mode, startTemplateId);
                _requested = request;
                _lastRequested = request;
            }
        }

        public GameSessionLaunchRequestData Consume()
        {
            lock (_sync)
            {
                GameSessionLaunchRequestData consumedRequest = _requested;
                _requested = new GameSessionLaunchRequestData(GameSessionLaunchMode.MainMenu, null);
                return consumedRequest;
            }
        }
    }
}
