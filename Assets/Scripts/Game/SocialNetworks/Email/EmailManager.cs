using System.Collections.Generic;
using Core;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using UniRx;

namespace Game.SocialNetworks.Email
{
    public class EmailManager : Singleton<EmailManager>
    {
        private readonly CompositeDisposable _disposables = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<EmailMessage>()
                .Subscribe(AddEmail)
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Clear();
        }

        public IEnumerable<Email> GetEmails()
        {
            foreach (var email in GameManager.Instance.Emails)
            {
                if (SpritesManager.Instance.TryGetByName(email.SpriteName, out var sprite))
                {
                    email.Sprite = sprite;
                }

                yield return email;
            }
        }

        public void MarkRead(Email email)
        {
            email.IsNew = false;
            MsgBroker.Instance.Publish(new ReadEmailMessage());
        }

        private static void AddEmail(EmailMessage msg)
        {
            var email = new Email
            {
                Title = msg.Title,
                TitleArgs = msg.TitleArgs,
                Content = msg.Content,
                ContentArgs = msg.ContentArgs,
                Sender = msg.Sender,
                Sprite = msg.Sprite,
                SpriteName = msg.Sprite.name,
                Date = TimeManager.Instance.DisplayNow,
                IsNew = true,

                MainAction = msg.mainAction,
                QuickAction = msg.quickAction
            };

            GameManager.Instance.Emails.Insert(0, email);
        }
    }
}