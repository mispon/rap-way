namespace RapWay.Application.Session
{
    public enum GameSessionLaunchMode
    {
        MainMenu = 0,
        Continue = 1,
        NewCareer = 2
    }

    public enum CareerStartTemplateId
    {
        OnYourOwn = 0,
        AtRockBottom = 1,
        PrivilegedStart = 2,
        OneMemeWonder = 3,
        BasementGenius = 4,
        FormerGroupMember = 5,
        Protege = 6
    }

    public readonly struct GameSessionLaunchRequestData
    {
        public GameSessionLaunchRequestData(GameSessionLaunchMode mode, CareerStartTemplateId? startTemplateId)
        {
            Mode = mode;
            StartTemplateId = startTemplateId;
        }

        public GameSessionLaunchMode Mode { get; }

        public CareerStartTemplateId? StartTemplateId { get; }
    }
}
