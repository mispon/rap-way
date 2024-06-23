using System.Collections.Generic;
using System.Linq;
using Core;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using ScriptableObjects;
using UniRx;
using UnityEngine;

namespace Game.SocialNetworks.Email
{
    public class EmailManager : Singleton<EmailManager>
    {
        [SerializeField] private ImagesBank imagesBank;

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

        public List<Email> GetEmails()
        {
            return GameManager.Instance.Emails.ToList();
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
                Title      = msg.Title,
                Content    = msg.Content,
                Sender     = msg.Sender,
                SpriteName = msg.SpriteName,
                Date       = TimeManager.Instance.DisplayNow,
                IsNew      = true,

                MainAction  = msg.mainAction,
                QuickAction = msg.quickAction
            };

            GameManager.Instance.Emails.Insert(0, email);
        }
    }
}