using Core.Localization;

namespace MessageBroker.Messages.Game
{
    public struct GameReadyMessage { }

    public struct LangChangedMessage
    {
        public GameLang Lang;
    }
}