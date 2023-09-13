﻿using Core;
using Game.Analyzers;
using Game.Pages.Eagler;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Album
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
            expIncome.text = $"+{settings.AlbumRewardExp}";
            
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
            PlayerManager.Instance.GiveReward(album.FansIncome, album.MoneyIncome, settings.AlbumRewardExp);
            ProductionManager.AddAlbum(album);
        }
        
        protected override void AfterPageClose()
        {
            SaveResult(_albumInfo);
            _albumInfo = null;
        }
    }
}