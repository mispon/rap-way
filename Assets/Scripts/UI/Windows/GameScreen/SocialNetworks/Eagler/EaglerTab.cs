using System.Collections.Generic;
using Core;
using Core.Analytics;
using Enums;
using Extensions;
using Game.SocialNetworks.Eagler;
using ScriptableObjects;
using TMPro;
using UI.Controls.ScrollViewController;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;
using Random = UnityEngine.Random;

namespace UI.Windows.GameScreen.SocialNetworks.Eagler
{
    public class EaglerTab : Tab
    {
        [Header("Personal")]
        [SerializeField] private Image avatar;
        [SerializeField] private TextMeshProUGUI realName;
        [SerializeField] private TextMeshProUGUI nickName;
        [SerializeField] private TextMeshProUGUI fans;
        [SerializeField] private ImagesBank      imagesBank;

        [Header("Feed")]
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button sendButton;
        [Space]
        [SerializeField] private ScrollViewController feed;
        [SerializeField] private GameObject template;

        [Header("Trends")]
        [SerializeField] private TextMeshProUGUI[] trends;

        private readonly List<EaglerCard> _feedItems = new();

        private void Start()
        {
            sendButton.onClick.AddListener(PostEagle);
        }

        protected override void BeforeOpen()
        {
            UpdateDesc();
            UpdateTrends();
            CreateFeed();
        }

        protected override void AfterOpen()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.EaglerOpened);
            HintsManager.Instance.ShowHint("tutorial_eagler", SocialNetworksTabType.Eagler);
        }

        protected override void AfterClose()
        {
            ClearFeed();
        }

        private void UpdateDesc()
        {
            var info = PlayerAPI.Data.Info;

            avatar.sprite = info.Gender == Gender.Male ? imagesBank.MaleAvatar : imagesBank.FemaleAvatar;
            realName.text = $"{info.FirstName} {info.LastName}";
            nickName.text = $"@{info.NickName}";
            fans.text     = PlayerAPI.Data.Fans.GetShort();
        }

        private void UpdateTrends()
        {
            var trendsList = EaglerManager.Instance.GetTrends();

            for (var i = 0; i < trendsList.Count; i++)
            {
                trends[i].text = $"#{trendsList[i]}";
            }
        }

        private void CreateFeed()
        {
            var eagles = EaglerManager.Instance.GetEagles();

            for (var i = 0; i < eagles.Count; i++)
            {
                var data = eagles[i];

                var eagle = feed.InstantiatedElement<EaglerCard>(template);
                eagle.Initialize(i + 1, data);

                _feedItems.Add(eagle);
            }

            feed.RepositionElements(_feedItems);
        }

        private void ClearFeed()
        {
            foreach (var eagle in _feedItems)
            {
                Destroy(eagle.gameObject);
            }

            _feedItems.Clear();
        }

        private void PostEagle()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            if (string.IsNullOrWhiteSpace(input.text))
            {
                return;
            }

            var nick      = PlayerAPI.Data.Info.NickName;
            var fansCount = PlayerAPI.Data.Fans;

            EaglerManager.Instance.CreateUserEagle(nick, input.text, GetLikes(fansCount));

            ClearFeed();
            CreateFeed();

            input.text = string.Empty;
        }

        private static int GetLikes(int fans)
        {
            var percent = fans switch
            {
                >= 10_000_000 => 1,
                >= 1_000_000  => 5,
                >= 100_000    => 10,
                >= 10_000     => 20,
                _             => 50
            };

            var likes  = fans.GetPercent(percent);
            var jitter = likes.GetPercent(20);

            likes = Random.Range(likes - jitter, likes + jitter);
            likes = Mathf.Max(likes, 0);

            return likes;
        }
    }
}