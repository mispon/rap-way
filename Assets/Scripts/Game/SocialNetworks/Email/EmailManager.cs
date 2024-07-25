using System.Collections.Generic;
using Core;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Labels;
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
            MsgBroker.Instance
                .Receive<PlayerSignLabelsContractMessage>()
                .Subscribe(UpdateLabelsContractEmails)
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
                Type = msg.Type,
                Title = msg.Title,
                TitleArgs = msg.TitleArgs,
                Content = msg.Content,
                ContentArgs = msg.ContentArgs,
                Sender = msg.Sender,
                Sprite = msg.Sprite,
                SpriteName = msg.Sprite != null ? msg.Sprite.name : "",
                Date = TimeManager.Instance.DisplayNow,
                IsNew = true,

                MainAction = msg.mainAction,
                QuickAction = msg.quickAction
            };

            GameManager.Instance.Emails.Insert(0, email);
        }

        private void UpdateLabelsContractEmails(PlayerSignLabelsContractMessage _)
        {
            foreach (var email in GameManager.Instance.Emails)
            {
                if (email.Type == Enums.EmailsType.LabelsContract)
                {
                    email.MainAction = null;
                    email.QuickAction = null;
                }
            }
        }
    }
}