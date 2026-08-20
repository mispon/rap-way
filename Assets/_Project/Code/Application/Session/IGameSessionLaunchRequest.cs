namespace RapWay.Application.Session
{
    public interface IGameSessionLaunchRequest
    {
        GameSessionLaunchMode LastRequestedMode { get; }

        void Request(GameSessionLaunchMode mode);

        GameSessionLaunchMode Consume();
    }
}
