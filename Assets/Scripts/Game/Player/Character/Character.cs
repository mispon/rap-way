using System.IO;
using CharacterCreator2D;
using Core;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;
using UnityEngine;

namespace Game.Player.Character
{
    public class Character : Singleton<Character>
    {
        private CharacterViewer _viewer;
        private CharacterData   _defaultData;

        private readonly CompositeDisposable _disposable = new();

        public CharacterViewer Viewer
        {
            get
            {
                if (_viewer == null)
                {
                    _viewer      = GetComponentInChildren<CharacterViewer>();
                    _defaultData = _viewer.GenerateCharacterData();
                }

                return _viewer;
            }
        }

        protected override void Initialize()
        {
            MsgBroker.Instance
                .Receive<WindowControlMessage>()
                .Subscribe(HandleWindow)
                .AddTo(_disposable);

            Load();
        }

        private void HandleWindow(WindowControlMessage m)
        {
            switch (m.Type)
            {
                case WindowType.GameScreen:
                    Hide();
                    break;

                case WindowType.CharacterCreator:
                    SetDefault();
                    break;

                case WindowType.MainMenu:
                    Load();
                    break;

                default:
                    SetPosition();
                    Show();
                    break;
            }
        }

        public void Show()
        {
            Viewer.gameObject.SetActive(true);
        }

        public void Show(float x, float y = -6.5f)
        {
            Show();
            SetPosition(x, y);
        }

        public void Hide()
        {
            Viewer.gameObject.SetActive(false);
        }

        private void SetDefault()
        {
            Viewer.AssignCharacterData(_defaultData);
        }

        private void SetPosition(float x = 0.0f, float y = -6.5f)
        {
            Viewer.transform.position = new Vector3(x, y, 0);
        }

        public void Save()
        {
            Viewer.SaveToJSON(GetFilePath());
        }

        private void Load()
        {
            if (Viewer.LoadFromJSON(GetFilePath()))
            {
                Viewer.Initialize();
            }
        }

        private static string GetFilePath()
        {
            return Path.Combine(Application.streamingAssetsPath, "character.json");
        }

        protected override void Dispose()
        {
            _disposable?.Clear();
        }
    }
}