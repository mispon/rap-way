using System;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game.Player;
using Game.Production;
using Game.Production.Analyzers;
using Game.Socials.Eagler;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using Models.Production;
using UI.Windows.GameScreen;
using UI.Windows.Pages.Eagler;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.Pages.Album
{
    /// <summary>
    /// Страница результатов альбома
    /// </summary>
    public class AlbumResultPage : Page
    {
        [Header("Компоменты")]
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

        [Header("Твитты фанатов")]
        [SerializeField] private EagleCard[] eagleCards;

        [Header("Анализатор альбома")]
        [SerializeField] private AlbumAnalyzer albumAnalyzer;

        private AlbumInfo _albumInfo;
        
        /// <summary>
        /// Показывает результат 
        /// </summary>
        public void Show(AlbumInfo album)
        {
            _albumInfo = album;
            
            albumAnalyzer.Analyze(album);
            DisplayResult(album);
            Open();
        }
        
        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(AlbumInfo album)
        {
            var nickname = PlayerManager.Data.Info.NickName;

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
            var eagles = EaglerManager.Instance.GenerateEagles(quality);
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
        
        protected override void AfterPageClose()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.AlbumResultShown);
            
            SaveResult(_albumInfo);
            _albumInfo = null;
        }
    }
}