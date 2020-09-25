using Core;
using Game.Analyzers;
using Game.UI.GameScreen;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

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
            header.text = $"Завершена работа над альбомом: \"{nickname} - {album.Name}\"";
            listenAmount.text = $"Количество прослушиваний: {album.ListenAmount}";
            chartInfo.text = album.ChartPosition > 0
                ? $"Альбом занял {album.ChartPosition}-ую позицию в чарте!"
                : "Альбом не попал в топ чарта";
            fansIncome.text = $"ФАНАТЫ: +{album.FansIncome}";
            moneyIncome.text = $"ДЕНЬГИ: +{album.MoneyIncome}$";
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