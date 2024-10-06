using System;
using Core.Analytics;
using Core.Context;
using Enums;
using Extensions;
using Game.Production;
using Game.Production.Analyzers;
using Game.SocialNetworks.Eagler;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using ScriptableObjects;
using UI.Windows.GameScreen.SocialNetworks.Eagler;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Album
{
    public class AlbumResultPage : Page
    {
        [SerializeField] private Text       listenAmount;
        [SerializeField] private Text       songs;
        [SerializeField] private Text       albumNameLabel;
        [SerializeField] private Text       playerNameLabel;
        [SerializeField] private Text       qualityLabel;
        [SerializeField] private Text       chartInfo;
        [SerializeField] private Text       fansIncome;
        [SerializeField] private Text       moneyIncome;
        [SerializeField] private Text       expIncome;
        [SerializeField] private GameObject hitBadge;

        [Header("Eagler")]
        [SerializeField] private EaglerCard[] eagleCards;

        [Header("Images")]
        [SerializeField] private ImagesBank imagesBank;

        private AlbumInfo _album;

        public override void Show(object ctx = null)
        {
            _album = ctx.Value<AlbumInfo>();

            AlbumAnalyzer.Analyze(_album, settings);
            DisplayResult(_album);

            base.Show(ctx);
        }

        private void DisplayResult(AlbumInfo album)
        {
            var nickname = PlayerAPI.Data.Info.NickName;

            albumNameLabel.text  = album.Name;
            playerNameLabel.text = nickname;

            fansIncome.text  = $"+{album.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{album.MoneyIncome.GetMoney()}";
            expIncome.text   = $"+{settings.Album.RewardExp}";

            qualityLabel.text = $"{Convert.ToInt32(album.Quality * 100)}%";
            listenAmount.text = album.ListenAmount.GetDisplay();
            songs.text        = $"{Random.Range(8, 31)}";

            hitBadge.SetActive(album.IsHit);

            chartInfo.text = album.ChartPosition > 0
                ? $"{album.ChartPosition}. {nickname} - {album.Name}"
                : GetLocale("album_result_chart_miss");

            DisplayEagles(album.Quality);
        }

        private void DisplayEagles(float quality)
        {
            var nickname = PlayerAPI.Data.Info.NickName;
            var fans     = PlayerAPI.Data.Fans;

            var eagles = EaglerManager.Instance.GenerateEagles(3, nickname, fans, quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
            }
        }

        private void SaveResult(AlbumInfo album)
        {
            album.Timestamp = TimeManager.Instance.Now.DateToString();
            ProductionManager.AddAlbum(album);

            MsgBroker.Instance.Publish(new ProductionRewardMessage
            {
                MoneyIncome = album.MoneyIncome,
                FansIncome  = album.FansIncome,
                Exp         = settings.Album.RewardExp
            });
        }

        protected override void AfterHide()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.AlbumResultShown);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_album_created",
                TextArgs = new[]
                {
                    PlayerAPI.Data.Info.NickName,
                    _album.Name
                },
                Sprite     = imagesBank.NewsAlbum,
                Popularity = PlayerAPI.Data.Fans
            });

            SaveResult(_album);
            _album = null;
        }
    }
}