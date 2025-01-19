using System.IO;
using CharacterCreator2D;
using Core;
using Core.Context;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Player.Character
{
    public class Character : Singleton<Character>
    {
        [SerializeField] private Part defaultMaleArmor;
        [SerializeField] private Part defaultMalePants;
        [SerializeField] private Part defaultFemaleArmor;
        [SerializeField] private Part defaultFemalePants;

        private CharacterViewer _viewer;
        private Animator        _animator;
        private CharacterData   _defaultData;

        private readonly CompositeDisposable _disposable = new();

        public CharacterViewer Viewer
        {
            get
            {
                if (_viewer == null)
                {
                    _viewer      = GetComponentInChildren<CharacterViewer>();
                    _animator    = _viewer.GetComponent<Animator>();
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
        }

        private void HandleWindow(WindowControlMessage m)
        {
            switch (m.Type)
            {
                case WindowType.GameScreen:
                    Hide();
                    break;

                case WindowType.CharacterCreator:
                    var reset = m.Context.Value<bool>();
                    if (reset)
                    {
                        SetDefaultData();
                    }

                    break;

                case WindowType.MainMenu:
                    Load();
                    break;

                case WindowType.Shop_Clothes:
                    SetPosition(0, -7.5f);
                    Show();
                    break;

                default:
                    SetPosition();
                    Show();
                    break;
            }
        }

        public void Load()
        {
            if (Viewer.LoadFromJSON(GetFilePath()))
            {
                Viewer.Initialize();
            } else
            {
                SetDefaultData();
            }
        }

        public void Show()
        {
            Viewer.gameObject.SetActive(true);
            _animator.speed = 1;
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

        public void SetAnimationSpeed(float speed)
        {
            _animator.speed = speed;
        }

        private void SetDefaultData()
        {
            Viewer.AssignCharacterData(_defaultData);
            DefaultClothes();
        }

        private void DefaultClothes()
        {
            // reset all clothes
            var slots = new[]
            {
                SlotCategory.Helmet, SlotCategory.Gloves,
                SlotCategory.Boots, SlotCategory.Skirt,
                SlotCategory.MainHand
            };
            foreach (var slot in slots)
            {
                Viewer.EquipPart(slot, "");
            }

            // default outwear
            Viewer.EquipPart(SlotCategory.Armor, Viewer.bodyType == BodyType.Male ? defaultMaleArmor : defaultFemaleArmor);
            Viewer.SetPartColor(SlotCategory.Armor, Color.white, Color.white, Color.white);

            // default pants 
            Viewer.EquipPart(SlotCategory.Pants, Viewer.bodyType == BodyType.Male ? defaultMalePants : defaultFemalePants);
            Viewer.SetPartColor(SlotCategory.Pants, Color.white, Color.white, Color.white);
        }

        public void ResetClothes()
        {
            DefaultClothes();

            var clothes = PlayerAPI.Inventory.GetEquippedClothes();

            foreach (var ci in clothes)
            {
                Viewer.EquipPart(ci.Slot, ci.Name);
                Viewer.SetPartColor(ci.Slot, ci.Color1, ci.Color2, ci.Color3);
            }
        }

        private void SetPosition(float x = 0.0f, float y = -6.5f)
        {
            Viewer.transform.position = new Vector3(x, y, 0);
        }

        public void Save()
        {
            Viewer.SaveToJSON(GetFilePath());
        }

        private static string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "character.json");
        }

        protected override void Dispose()
        {
            _disposable?.Clear();
        }
    }
}