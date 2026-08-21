using System.Collections.Generic;

namespace RapWay.Presentation.Unity.Localization
{
    public static class GameLocalizationSeedData
    {
        private static readonly IReadOnlyList<GameLocalizationSeedEntry> EntriesList = new[]
        {
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.BrandTitle, "Rap Way", "Rap Way"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.SplashPreparingStage, "Готовим сцену...", "Preparing the stage..."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.MainMenuSubtitle, "Построй своё имя с самого низа.", "Build your name from the ground up."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.MainMenuNewCareer, "Новая карьера", "New Career"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.MainMenuContinue, "Продолжить", "Continue"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.MainMenuQuit, "Выход", "Quit"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.MainMenuStatusCanContinue, "Выбирай следующий шаг.", "Pick your next move."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.MainMenuStatusStartFirstCareer, "Начни свою первую карьеру.", "Start your first career."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateScreenTitle, "Выбери старт", "Choose Your Start"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateScreenSubtitle, "Шаблоны меняют стартовые обстоятельства, но не правила симуляции.", "Templates change your opening circumstances, not the core simulation rules."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateBack, "Назад", "Back"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateConfirm, "Начать карьеру", "Start Career"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateStatusChooseCircumstances, "Выбери, в каких обстоятельствах начнётся твоя карьера.", "Choose the circumstances your career begins with."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.HudTitle, "Заглушка игрового HUD", "Session HUD Placeholder"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.HudBody, "Shell Stage 4 активен.", "Stage 4 shell is active."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.HudMenu, "Меню", "Menu"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.ModalExitTitle, "Выйти из Rap Way?", "Exit Rap Way?"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.ModalExitBody, "Закрыть приложение сейчас.", "Close the app now."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.ModalExitConfirm, "Выйти", "Exit"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.ModalSessionTitle, "Меню сессии", "Session Menu"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.ModalSessionBody, "Вернуться в главное меню.", "Return to the main menu scene."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.ModalSessionConfirm, "Главное меню", "Main Menu"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.ModalClose, "Закрыть", "Close"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.StatusReturningToMainMenu, "Возвращаемся в главное меню...", "Returning to the main menu..."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.StatusStartingNewCareer, "Запускаем старт «{templateTitle}»...", "Starting {templateTitle}..."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.StatusLoadingLatestCareer, "Загружаем последнюю карьеру...", "Loading your latest career..."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.StatusGenericError, "Что-то пошло не так. Проверь консоль.", "Something went wrong. Check the console."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.HudStatusCareerSessionRunning, "Игровая сессия запущена.", "Career session running."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOnYourOwnTitle, "Сам по себе", "On Your Own"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOnYourOwnTagline, "Чистый лист и никакой подушки.", "Clean slate, no safety net."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOnYourOwnSummary, "Ты начинаешь без имени, без денег и без чужого багажа. Каждый шаг придётся вытащить самому.", "You start unknown, underfunded, and free from baggage. Every bit of progress is yours to earn."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOnYourOwnEmphasis, "Сбалансированный базовый старт для первой карьеры.", "Balanced baseline for a first career run."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateAtRockBottomTitle, "На дне", "At Rock Bottom"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateAtRockBottomTagline, "Долги, голод и никакого стабильного дома.", "Debt, hunger, and no stable home."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateAtRockBottomSummary, "Сначала придётся выживать и искать быстрые деньги, прежде чем музыка станет чем-то большим, чем мечта.", "You begin in survival mode and need fast cash before music can become more than a dream."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateAtRockBottomEmphasis, "Хардкорный старт с самым жёстким давлением в начале.", "Hardcore recovery route with the harshest early pressure."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplatePrivilegedStartTitle, "Привилегированный старт", "Privileged Start"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplatePrivilegedStartTagline, "Деньги и связи есть, уважения нет.", "Money and access, little respect."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplatePrivilegedStartSummary, "Некоторые двери открыты заранее, но от тебя ждут результата и считают, что ты ничего не добился сам.", "Doors open faster, but everyone expects results and assumes you did not earn your shot."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplatePrivilegedStartEmphasis, "Более безопасная экономика, но сильнее социальное давление.", "Safer economy with more social scrutiny."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOneMemeWonderTitle, "Мем на один раз", "One-Meme Wonder"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOneMemeWonderTagline, "Вспышка хайпа без фундамента.", "A burst of hype with no foundation."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOneMemeWonderSummary, "Твоё имя уже мелькнуло, но не так, как хотелось бы. Нужно быстро превратить внимание в настоящих фанатов.", "People know your name for the wrong reason. You must convert attention into real fans before it fades."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateOneMemeWonderEmphasis, "Рискованный старт, завязанный на скоротечный хайп.", "Volatile high-risk opening built around hype decay."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateBasementGeniusTitle, "Подвальный гений", "Basement Genius"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateBasementGeniusTagline, "Сильный крафт, слабая сцена.", "Serious craft, weak stage presence."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateBasementGeniusSummary, "Ты уже умеешь делать хорошую музыку, но выступления, связи и образ тянут назад.", "You already know how to make strong music, but performing, networking, and image are holding you back."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateBasementGeniusEmphasis, "Старт через навыки с заметными социальными слабостями.", "Skill-forward opening with social weaknesses."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateFormerGroupMemberTitle, "Бывший участник группы", "Former Group Member"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateFormerGroupMemberTagline, "Аудитория есть, но история грязная.", "An audience, a breakup, and messy history."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateFormerGroupMemberSummary, "За спиной остались совместные треки, обиды и спорные права. Часть публики с тобой, часть — против.", "You leave a past collective with some fans and some enemies. Old rights and resentments still follow you."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateFormerGroupMemberEmphasis, "Старт с инерцией и конфликтами в отношениях.", "Starts with momentum and relationship complications."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateProtegeTitle, "Протеже", "Protégé"),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateProtegeTagline, "Мощный наставник и длинная тень.", "A powerful mentor and a long shadow."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateProtegeSummary, "Тебя поддерживает известный артист, а вместе с этим приходят доступ, давление и постоянные сравнения.", "An established artist backs you, giving access and pressure in equal measure. People compare every move."),
            new GameLocalizationSeedEntry(GameLocalizationKeys.UiShell.TemplateProtegeEmphasis, "Продвинутый сюжетный старт с унаследованными ожиданиями.", "Advanced narrative start with inherited expectations.")
        };

        public static IReadOnlyList<GameLocalizationSeedEntry> Entries
        {
            get
            {
                return EntriesList;
            }
        }

        public static bool TryGet(LocalizationKey key, out GameLocalizationSeedEntry entry)
        {
            for (int index = 0; index < EntriesList.Count; index++)
            {
                GameLocalizationSeedEntry current = EntriesList[index];
                if (!current.Key.Equals(key))
                {
                    continue;
                }

                entry = current;
                return true;
            }

            entry = default;
            return false;
        }
    }

    public readonly struct GameLocalizationSeedEntry
    {
        public GameLocalizationSeedEntry(LocalizationKey key, string russianText, string englishText)
        {
            Key = key;
            RussianText = russianText ?? string.Empty;
            EnglishText = englishText ?? string.Empty;
            IsSmart = ContainsPlaceholder(RussianText) || ContainsPlaceholder(EnglishText);
        }

        public LocalizationKey Key { get; }

        public string RussianText { get; }

        public string EnglishText { get; }

        public bool IsSmart { get; }

        private static bool ContainsPlaceholder(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Contains("{") && value.Contains("}");
        }
    }
}
