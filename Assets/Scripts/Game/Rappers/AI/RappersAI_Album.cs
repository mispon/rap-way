using System;
using Core.Analytics;
using Enums;
using Game.Production.Analyzers;
using Game.Rappers.Desc;
using Game.Settings;
using Game.SocialNetworks.Eagler;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using Models.Trends;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private static void DoAlbum(RapperInfo rapper, GameSettings settings)
        {
            Debug.Log($"[RAPPER AI] {rapper.Name} do album");
            AnalyticsManager.LogEvent(FirebaseGameEvents.RapperAI_CreateAlbum);

            rapper.Cooldown = settings.Rappers.AlbumCooldown;

            var album = new AlbumInfo
            {
                CreatorId  = rapper.Id,
                Name       = GenAlbumName(),
                TextPoints = GenWorkPoints(rapper.Vocobulary, settings.Album.WorkDuration),
                BitPoints  = GenWorkPoints(rapper.Bitmaking, settings.Album.WorkDuration),
                TrendInfo = new TrendInfo
                {
                    Style = (Styles) Random.Range(0, Enum.GetValues(typeof(Styles)).Length),
                    Theme = (Themes) Random.Range(0, Enum.GetValues(typeof(Themes)).Length)
                }
            };

            AlbumAnalyzer.Analyze(album, settings);

            rapper.Fans = Math.Max(MIN_FANS_COUNT, rapper.Fans + album.FansIncome);
            rapper.History.AlbumList.Add(album);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_album_created",
                TextArgs   = new[] {rapper.Name, album.Name},
                Sprite     = rapper.Avatar,
                Popularity = rapper.Fans
            });

            EaglerManager.Instance.GenerateEagles(1, rapper.Name, rapper.Fans, album.Quality);
        }

        private static string GenAlbumName()
        {
            string[] albumNames =
            {
                "Hustler's Ambition", "King of the Streets", "Trap God Chronicles", "Money Power Respect", "Street Legends",
                "No Cap Dreams", "Rise of a Hustler", "Millionaire Grind", "Block Royalty", "From the Bottom to the Top", "Thug Life Story",
                "The Come Up", "Ghetto Dreams", "Back to the Trap", "Boss Moves", "Trap House Diaries", "Sky's the Limit", "No Days Off",
                "Heart of a Hustler", "Ambitionz of a King", "Trap Symphony", "Street Tales", "No Surrender", "Street Soldier Chronicles",
                "Get Rich or Try", "Flex on the World", "No Handouts", "Money Talks", "Hood Royalty", "Savage Mode 2",
                "Grinding in Silence", "Ruthless Hustle", "Cold World Chronicles", "Street Code", "Made for This", "Stackin' Bands",
                "Out the Mud", "Block Life", "Dreams of the Trap", "No Limit Life", "Thug Motivation", "Real Talk Stories",
                "Trap House Hustle", "Block Party Vibes", "Million Dollar Hustle", "Grit and Grind", "From the Ground Up",
                "Trap Kings and Queens", "Money Dreams", "Survivor's Story", "No Love Lost", "Boss Mentality", "Street Scriptures",
                "Out the Trenches", "Cash King Chronicles", "Real One Anthem", "Game Changer", "Stay Focused", "Heart of Gold",
                "Trap Money Diaries", "No Sleep 'Til Success", "Loyalty and Respect", "Money Moves", "Street Runner", "Thug Life Forever",
                "Born to Ball", "Hustle & Heart", "In My Bag", "Trill Life Chronicles", "City of Dreams", "Fast Life Anthems",
                "Racks and Stacks", "Bread Winners", "Hood Dreams", "Back on the Block", "Ten Toes Down", "Bossed Up", "Trap Chronicles",
                "Real Hustlers Only", "No Love in the Streets", "Stay Hungry Stay Humble", "Cash Out Dreams", "Trap Lord Status",
                "Grind Till I Shine", "Money Over Fame", "Hustle Never Sleeps", "Street Dreams 2", "Born Hustler", "Bag Season Chronicles",
                "No Limit Ambition", "Trap House Legends", "Get That Bag", "Street Life Stories", "Back to the Streets",
                "Dreams & Nightmares", "Winning Streak", "Street Hustler Diaries", "Stay in Your Lane", "Money in Motion",
                "Street Politics", "Made for the Streets", "Grind Mode Chronicles", "Pressure Makes Diamonds", "Born to Win",
                "From the Hood to the Stage", "Stack It Up", "Trap God Legacy", "Ghetto Gospel", "Real Spitta Anthems", "Thug Royalty",
                "Money on My Mind", "Street Hustle Forever", "Out the Gutter", "Ambitionz as a Hustler", "No Surrender Chronicles",
                "Money Mission", "Trap Chronicles 2", "Block Work Diaries", "No Hook Needed", "Stay Real", "Street Code 2",
                "Money Train Anthems", "Loyal to the Block", "Dream Chasers", "Boss Moves Only", "From the Block to the Top", "Hood Ties",
                "All Gas No Brakes", "Street Knowledge Chronicles", "Trappin' Ain't Dead", "On the Come Up", "Heart of the Streets",
                "Street Soldier Stories", "Street Hustle Vol. 1", "Bag Chasin' Dreams", "Get Paid or Die Tryin'", "Million Dollar Dreams",
                "Block Boy Anthem", "Fast Lane Living", "Trap Dreams 2", "Born to Hustle", "Back to Business", "Winning Team",
                "Cash Rules Everything", "Street Hustle Diaries", "Trap Symphony 2", "Pressure Chronicles", "No Cap Stories",
                "Money Over Everything", "Hustler's Prayer", "Street Kings and Queens", "Get It Out the Mud", "Real One Chronicles",
                "Back on the Block 2", "Hood Rich Anthems", "Trap or Die", "Grind to Shine", "Thug World Order", "Stacked Up",
                "Trap Life Diaries", "Out the Trap", "No Days Off Vol. 2", "Hood Royalty Chronicles", "Street Life Vol. 2",
                "Stay Real Stories", "Ambitionz of a Hustler", "Fast Money Dreams", "Money Power Moves", "From the Streets to the Throne",
                "Trap House Anthems", "Rags to Riches", "Get Paid Chronicles", "No Hook Vol. 2", "From the Ground Up 2",
                "Boss Mentality Vol. 2", "Hustle Till I Die", "Trill Life Stories", "Street Dreams Vol. 2", "Made for This Vol. 2",
                "Grind Hard Chronicles", "Trap Star Stories", "Cold World Anthems", "Loyalty Over Everything", "Street Code Vol. 3",
                "Money Power Respect 2", "Hood Ties Vol. 2", "Trap Kings Vol. 2", "Stay Hungry Vol. 2", "Trap Chronicles Vol. 3",
                "From the Trenches", "Dreams and Ambitions", "Get Rich or Die Tryin' Vol. 2", "Money Mission Vol. 2", "Boss Moves Vol. 3",
                "Thug Motivation Vol. 2", "Street Politics Vol. 2", "No Cap Vol. 3", "Real Hustlers Chronicles", "Savage Mode Chronicles",
                "Street Knowledge Vol. 2"
            };

            return albumNames[Random.Range(0, albumNames.Length)];
        }
    }
}