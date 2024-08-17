using System.Collections.Generic;
using System.Linq;
using Enums;
using Firebase.Analytics;
using Game.SocialNetworks.Email;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using UI.Controls.ScrollViewController;
using UI.Windows.Tutorial;
using UniRx;
using UnityEngine;
using EmailInfo = Game.SocialNetworks.Email.Email;

namespace UI.Windows.GameScreen.SocialNetworks.Email
{
    public class EmailTab : Tab
    {
        [SerializeField] private ScrollViewController feed;
        [SerializeField] private GameObject template;
        [SerializeField] private EmailContentView textTemplate;
        [SerializeField] private EmailContentView imageTemplate;
        [SerializeField] private GameObject emptyListIcon;
        [SerializeField] private GameObject emptyEmailsIcon;

        private readonly CompositeDisposable _disposables = new();
        private readonly List<EmailCard> _emailCards = new();
        private EmailCard _lastSelected;

        protected override void BeforeOpen()
        {
            var emails = EmailManager.Instance.GetEmails().ToArray();

            for (var i = 0; i < emails.Length; i++)
            {
                var data = emails[i];

                var card = feed.InstantiatedElement<EmailCard>(template);
                card.Initialize(i + 1, data, HandleClick);

                _emailCards.Add(card);
            }

            feed.RepositionElements(_emailCards);

            MsgBroker.Instance
                .Receive<EmailMessage>()
                .Subscribe(AppendEmail)
                .AddTo(_disposables);
        }

        protected override void AfterOpen()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.EmailOpened);
            HintsManager.Instance.ShowHint("tutorial_email", SocialNetworksTabType.Email);

            var hasEmails = _emailCards.Any();
            if (hasEmails)
            {
                _emailCards[0].Open();
            }

            emptyListIcon.SetActive(!hasEmails);
            emptyEmailsIcon.SetActive(!hasEmails);
        }

        private void HandleClick(EmailCard card, EmailInfo email)
        {
            if (card == _lastSelected)
                return;

            if (_lastSelected != null)
            {
                _lastSelected.Unselect();
            }
            _lastSelected = card;

            if (email.Sprite == null)
            {
                textTemplate.ShowText(email);
                imageTemplate.Hide();
            }
            else
            {
                imageTemplate.ShowImage(email);
                textTemplate.Hide();
            }

            if (email.IsNew)
            {
                EmailManager.Instance.MarkRead(email);
            }
        }

        private void AppendEmail(EmailMessage msg)
        {
            var email = EmailManager.Instance.ConvertMessage(msg);

            var card = feed.InstantiatedElement<EmailCard>(template);
            card.Initialize(_emailCards.Count + 1, email, HandleClick);

            _emailCards.Add(card);
            feed.RepositionElements(_emailCards);
        }

        protected override void AfterClose()
        {
            foreach (var email in _emailCards)
            {
                Destroy(email.gameObject);
            }

            _disposables.Clear();
            _emailCards.Clear();

            textTemplate.Hide();
            imageTemplate.Hide();
        }
    }
}