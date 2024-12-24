namespace UI.Enums
{
    public enum WindowType
    {
        // Main menu
        MainMenu         = 0,
        About            = 1,
        AskReview        = 2,
        Settings         = 3,
        LangSelection    = 4,
        CharacterCreator = 5,
        PlayerInfo       = 6,
        _main_menu       = 19,

        // Game
        GameScreen      = 20,
        GameEvent       = 21,
        GameFinish      = 22,
        GameEventResult = 23,
        _game           = 39,

        // Production
        ProductionSelect = 40,

        // Track
        ProductionTrackSettings = 41,
        ProductionTrackWork     = 42,
        ProductionTrackResult   = 43,
        _prod_track             = 49,

        // Album
        ProductionAlbumSettings = 50,
        ProductionAlbumWork     = 51,
        ProductionAlbumResult   = 52,
        _prod_album             = 59,

        // Clip
        ProductionClipSettings = 60,
        ProductionClipWork     = 61,
        ProductionClipResult   = 62,
        _prod_clip             = 69,

        // Concert
        ProductionConcertSettings = 70,
        ProductionConcertWork     = 71,
        ProductionConcertResult   = 72,
        _prod_concert             = 79,

        // Feat
        ProductionFeatSettings = 80,
        ProductionFeatWork     = 81,
        ProductionFeatResult   = 82,
        _prod_feat             = 89,

        _other_production_reserve = 99,

        // Shop
        Shop               = 100,
        Shop_ItemCard      = 101,
        Shop_PurchasedItem = 102,
        Shop_Clothes       = 103,
        _shop              = 149,

        // Battle
        BattleWork   = 150,
        BattleResult = 151,
        _battle      = 159,

        // Socials
        SocialNetworks          = 161,
        SocialsActions          = 24,
        SocialsWork             = 25,
        SocialsResult_Charity   = 26,
        SocialsResult_Eagler    = 27,
        SocialsResult_Ieyegram  = 28,
        SocialsResult_Switch    = 29,
        SocialsResult_TackTack  = 30,
        SocialsResult_Telescope = 31,
        SocialsResult_Trends    = 32,
        _socials                = 199,

        // Single Pages
        Personal            = 200,
        Training            = 201,
        StatsDesc           = 202,
        Hints               = 203,
        Charts              = 204,
        History             = 205,
        TeammateUnlocked    = 206,
        AchievementUnlocked = 207,
        _single_pages       = 249,

        // Rappers
        RapperConversationsWork   = 250,
        RapperConversationsResult = 251,
        RapperJoinLabel           = 252,
        NewRapper                 = 253,
        _rappers                  = 274,

        // Labels
        NewLabel      = 275,
        LabelContract = 276,
        _labels       = 299,

        // System
        Previous = 998,
        None     = 999
    }
}