using System;
using Core.Context;
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
using Sirenix.OdinInspector;
using UI.Windows.GameScreen.Eagler;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.GameScreen.Album
{
    /// <summary>
    /// Страница результатов альбома
    /// </summary>
    public class AlbumResultPage : Page
    {
        [BoxGroup("Result"), SerializeField] private Text listenAmount;
        [BoxGroup("Result"), SerializeField] private Text songs;
        [BoxGroup("Result"), SerializeField] private Text albumNameLabel;
        [BoxGroup("Result"), SerializeField] private Text playerNameLabel;
        [BoxGroup("Result"), SerializeField] private Text qualityLabel;
        [BoxGroup("Result"), SerializeField] private Text chartInfo;
        [BoxGroup("Result"), SerializeField] private Text fansIncome;
        [BoxGroup("Result"), SerializeField] private Text moneyIncome;
        [BoxGroup("Result"), SerializeField] private Text expIncome;
        [BoxGroup("Result"), SerializeField] private GameObject hitBadge;
        
        [BoxGroup("Eagler"), SerializeField] private EagleCard[] eagleCards;
        [BoxGroup("Analyzer"), SerializeField] private AlbumAnalyzer albumAnalyzer;

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
        
        protected override void AfterHide()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.AlbumResultShown);
            
            SaveResult(_album);
            _album = null;
        }
    }
}