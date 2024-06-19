using System;
using Core.Context;
using Extensions;
// using Firebase.Analytics;
using Game.Production;
using Game.Production.Analyzers;
using Game.Socials.Twitter;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using Models.Production;
using UI.Windows.GameScreen.Twitter;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Album
{
    public class AlbumResultPage : Page
    {
        [Header("Result")]
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text songs;
        [SerializeField] private Text albumNameLabel;
        [SerializeField] private Text playerNameLabel;
        [SerializeField] private Text qualityLabel;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;
        [SerializeField] private GameObject hitBadge;
        
        [Header("Eagler"), SerializeField] private TwitterCard[] eagleCards;
        [Header("Analyzer"), SerializeField] private AlbumAnalyzer albumAnalyzer;

        private AlbumInfo _album;
        
        public override void Show(object ctx = null)
        {
            _album = ctx.Value<AlbumInfo>();
            
            albumAnalyzer.Analyze(_album);
            DisplayResult(_album);
            
            base.Show(ctx);
        }
        
        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(AlbumInfo album)
        {
            var nickname = PlayerAPI.Data.Info.NickName;

            albumNameLabel.text = album.Name;
            playerNameLabel.text = nickname;
            
            fansIncome.text = $"+{album.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{album.MoneyIncome.GetMoney()}";
            expIncome.text = $"+{settings.Album.RewardExp}";

            qualityLabel.text = $"{Convert.ToInt32(album.Quality * 100)}%";
            listenAmount.text = album.ListenAmount.GetDisplay();
            songs.text = $"{Random.Range(8, 31)}";
            
            hitBadge.SetActive(album.IsHit);
            
            chartInfo.text = album.ChartPosition > 0
                ? $"{album.ChartPosition}. {nickname} - {album.Name}"
                : GetLocale("album_result_chart_miss");
            
            DisplayEagles(album.Quality);
        }
        
        private void DisplayEagles(float quality)
        {
            var eagles = TwitterManager.Instance.GenerateTwits(quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
            }
        }

        /// <summary>
        /// Сохраняет результаты альбома
        /// </summary>
        private void SaveResult(AlbumInfo album)
        {
            album.Timestamp = TimeManager.Instance.Now.DateToString();
            ProductionManager.AddAlbum(album);
            
            MsgBroker.Instance.Publish(new ProductionRewardMessage
            {
                MoneyIncome = album.MoneyIncome,
                FansIncome = album.FansIncome,
                Exp = settings.Album.RewardExp
            });
        }
        
        protected override void AfterHide()
        {
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.AlbumResultShown);
            
            SaveResult(_album);
            _album = null;
        }
    }
}