using Core;
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
        [SerializeField] private int rewardExp;

        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;

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
            header.text = $"Работа над альбомом <color=#01C6B8>\"{nickname} - {album.Name}\"</color> завершена";
            listenAmount.text = $"Альбом послушали <color=#F6C326>{album.ListenAmount}</color> раз!";
            chartInfo.text = album.ChartPosition > 0
                ? $"Альбом занял <color=#00F475>{album.ChartPosition}</color> позицию в чарте!"
                : "Альбом не попал в топ чартов";
            fansIncome.text = $"+{album.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{album.MoneyIncome.GetMoney()}";
        }

        /// <summary>
        /// Сохраняет результаты альбома
        /// </summary>
        private void SaveResult(AlbumInfo album)
        {
            album.Timestamp = TimeManager.Instance.Now;
            PlayerManager.Instance.GiveReward(album.FansIncome, album.MoneyIncome, rewardExp);
            ProductionManager.AddAlbum(album);
        }
        
        protected override void AfterPageClose()
        {
            SaveResult(_albumInfo);
            _albumInfo = null;
        }
    }
}