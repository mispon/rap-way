﻿using Core;
using Game.Analyzers;
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
        [SerializeField] private Text header;
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;

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
            DisplayEagles(album.Quality);
            
            var nickname = PlayerManager.Data.Info.NickName;
            header.text = GetLocale("album_result_header", nickname, album.Name);
            listenAmount.text = GetLocale("album_result_listens", album.ListenAmount.GetDisplay());
            chartInfo.text = album.ChartPosition > 0
                ? GetLocale("album_result_chart_pos", album.ChartPosition)
                : GetLocale("album_result_chart_miss");
            fansIncome.text = $"+{album.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{album.MoneyIncome.GetMoney()}";
            expIncome.text = $"+{settings.AlbumRewardExp}";
        }
        
        private void DisplayEagles(float quality)
        {
            var eagles = EaglerManager.Instance.GenerateEagles(quality);
            foreach (var eagle in eagles)
            {
                Debug.Log(eagle);
                // todo
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