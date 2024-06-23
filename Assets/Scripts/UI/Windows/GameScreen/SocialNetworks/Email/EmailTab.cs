using System.Collections.Generic;
using Game.SocialNetworks.Email;
using UI.Controls.ScrollViewController;
using UnityEngine;
using EmailInfo = Game.SocialNetworks.Email.Email;

namespace UI.Windows.GameScreen.SocialNetworks.Email
{
    public class EmailTab : Tab
    {
        [SerializeField] private ScrollViewController feed;
        [SerializeField] private GameObject           template;
        [SerializeField] private EmailContentView     textTemplate;
        [SerializeField] private EmailContentView     imageTemplate;

        private readonly List<EmailCard> _emailCards = new();

        private EmailCard _lastSelected;

        protected override void BeforeOpen()
        {
            var emails = EmailManager.Instance.GetEmails();

            for (var i = 0; i < emails.Count; i++)
            {
                var data = emails[i];

                var card = feed.InstantiatedElement<EmailCard>(template);
                card.Initialize(i + 1, data, HandleClick);

                _emailCards.Add(card);
            }

            feed.RepositionElements(_emailCards);
        }

        private void HandleClick(EmailCard card, EmailInfo email)
        {
            if (card == _lastSelected)
            {
                return;
            }

            if (_lastSelected != null)
            {
                _lastSelected.Unselect();
            }

            _lastSelected = card;

            if (string.IsNullOrEmpty(email.SpriteName))
            {
                textTemplate.ShowText(email);
                imageTemplate.Hide();
            } else
            {
                imageTemplate.ShowImage(email);
                textTemplate.Hide();
            }

            if (email.IsNew)
            {
                EmailManager.Instance.MarkRead(email);
            }
        }

        protected override void AfterOpen()
        {
            // HintsManager.Instance.ShowHint("tutorial_email");
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.TwitterOpened);
        }

        protected override void AfterClose()
        {
            foreach (var email in _emailCards)
            {
                Destroy(email.gameObject);
            }

            _emailCards.Clear();

            textTemplate.Hide();
            imageTemplate.Hide();
        }
    }
}