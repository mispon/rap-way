namespace RapWay.Application.Session
{
    public interface IGameSessionLaunchRequest
    {
        GameSessionLaunchMode LastRequestedMode { get; }

        CareerStartTemplateId? LastRequestedStartTemplateId { get; }

        void Request(GameSessionLaunchMode mode, CareerStartTemplateId? startTemplateId = null);

        GameSessionLaunchRequestData Consume();
    }
}
