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
        private static void DoTrack(RapperInfo rapper, GameSettings settings)
        {
            Debug.Log($"[RAPPER AI] {rapper.Name} do track");
            AnalyticsManager.LogEvent(FirebaseGameEvents.RapperAI_CreateTrack);

            rapper.Cooldown = settings.Rappers.TrackCooldown;

            var track = new TrackInfo
            {
                CreatorId  = rapper.Id,
                FeatId     = TryDoFeat(rapper, settings.Rappers.FeatChance),
                Name       = GenTrackName(),
                TextPoints = GenWorkPoints(rapper.Vocobulary, settings.Track.WorkDuration),
                BitPoints  = GenWorkPoints(rapper.Bitmaking, settings.Track.WorkDuration),
                TrendInfo = new TrendInfo
                {
                    Style = (Styles) Random.Range(0, Enum.GetValues(typeof(Styles)).Length),
                    Theme = (Themes) Random.Range(0, Enum.GetValues(typeof(Themes)).Length)
                }
            };

            TrackAnalyzer.Analyze(track, settings);

            rapper.Fans = Math.Max(MIN_FANS_COUNT, rapper.Fans + track.FansIncome);
            rapper.History.TrackList.Add(track);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text       = "news_track_created",
                TextArgs   = new[] {rapper.Name, track.Name},
                Sprite     = rapper.Avatar,
                Popularity = rapper.Fans
            });

            EaglerManager.Instance.GenerateEagles(1, rapper.Name, rapper.Fans, track.Quality);
        }

        private static string GenTrackName()
        {
            string[] trackNames =
            {
                "Street Dreams", "Hustle Anthem", "Real Talk", "Paper Chasin'", "Block Party", "Money Machine", "Ride or Die", "Grind Hard",
                "King of the Block", "Stackin' Racks", "Thug Motivation", "Trap God", "Sky's the Limit", "Boss Moves", "Countin' Up",
                "Flex On 'Em", "Run the City", "No Cap", "Pull Up", "Ghetto Symphony", "Rollin' in the Deep", "Respect the Hustle",
                "Back on the Block", "Millionaire Mindset", "Trap House Dreams", "Street Code", "Born to Win", "Grind Mode",
                "All or Nothing", "Ghetto Gospel", "Hustler's Prayer", "No Love Lost", "Block Life", "Out the Mud", "Money Talks",
                "Fast Life", "Street Royalty", "Dreams of a Hustler", "Kingpin Status", "No Limit", "Savage Mode", "Cash Rules",
                "In My Bag", "Ambitionz", "Trap or Die", "Get Rich or Try", "Street Kings", "From the Bottom", "No Handouts",
                "Trap Symphony", "City of Dreams", "Hood Legend", "Stacks and Bands", "Make It Happen", "Real Recognize Real",
                "Stay in Your Lane", "Trappin' Ain't Dead", "Heart of a Hustler", "Ten Toes Down", "Run It Up", "Boss Up",
                "Hustle Till I Die", "Ballin' Out", "Million Dollar Dreams", "Made Men", "Cold World", "Talk My Sh*t",
                "Pressure Makes Diamonds", "On My Level", "In the Trap", "Stack It Up", "No Surrender", "Money Over Everything",
                "Hood Dreams", "Flexing Hard", "City Lights", "Street Politics", "Bag Season", "Heart of the Streets", "New Wave",
                "Grit and Grind", "Money Dance", "Loyal to the Block", "On the Come Up", "Game Changer", "Real Hustler", "Born to Ball",
                "From the Trenches", "Bread Winner", "Cash Flow", "Trap Life", "No Sleep", "Winning Streak", "King of the Streets",
                "Money Moves", "Pressure", "Out the Gutter", "Street Dreams 2", "My Way", "Paid in Full", "Real Life", "Bands on Deck",
                "Trap Chronicles", "Never Fold", "Heart of Gold", "Street Soldier", "Level Up", "No Days Off", "Dope Boy Dreams",
                "Cash King", "On My Grind", "Trap Star", "Money Mission", "Street Runner", "Stay Real", "Back to the Streets",
                "Making Moves", "Young and Paid", "Bag Chasin'", "Rags to Riches", "Boss Mentality", "Ruthless", "Game Plan",
                "Hustler's Dream", "Money Wave", "King of the Trap", "Back to Business", "On a Mission", "Still Hustlin'", "Trap Money",
                "Straight to the Top", "Real One", "Stay Focused", "Cash Out", "Ambitionz as a Ridah", "Hustle Mentality", "Street Hustler",
                "Pushing Weight", "From the Ground Up", "Trill Life", "Stay Hungry", "No Love", "Keep It Real", "Money Power Respect",
                "Still Ballin'", "Million Dollar Mind", "Made for This", "On the Block", "Top Dawg", "Dream Chaser",
                "Loyalty Over Everything", "Hood Rich", "Street Life", "Winning Team", "Cash in Hand", "Grinding All Day",
                "Forever Hustling", "Trap Lord", "Gutta Talk", "Money Over Fame", "Real Spitta", "Block Work", "Success Story",
                "Hood Royalty", "Street Runnin'", "Get That Bag", "Trap King", "Gold Chain Dreams", "Bag Secure", "Against All Odds",
                "Trap Chronicles 2", "Born Hustler", "Never Back Down", "Game On", "Run the Streets", "Street Code 2", "Hustle to Survive",
                "Get Paid", "Grind Don't Stop", "Fast Lane", "Money Train", "Back to the Trap", "Boss Talk", "Out the Trenches",
                "Stay Solid", "No Hook", "Hood Ties", "Street Knowledge", "Legend in the Making", "Survivor's Story",
                "Dreams and Nightmares", "Money Mindset", "All Gas No Brakes", "From the Streets to the Stage", "Grind to Shine",
                "Block Boy", "Trap Dreams", "Thug Life", "Stay in the Trap", "Real to the Bone"
            };

            return trackNames[Random.Range(0, trackNames.Length)];
        }
    }
}