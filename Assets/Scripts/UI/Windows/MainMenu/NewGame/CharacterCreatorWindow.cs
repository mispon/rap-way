using System;
using CharacterCreator2D;
using Core;
using Core.Context;
using Game.Player.Character;
using MessageBroker;
using MessageBroker.Messages.Player;
using ScriptableObjects;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu.NewGame
{
    [Serializable]
    public class CategoryRow
    {
        public Button     Button;
        public GameObject Outline;
        public GameObject Category;
        public GameObject CategoryColors;
        public bool       ZoomCharacter;
    }

    public class CharacterCreatorWindow : CanvasUIElement
    {
        [SerializeField] private Camera     mainCamera;
        [SerializeField] private ImagesBank imagesBank;

        [Header("Gender")]
        [SerializeField] private Button maleButton;
        [SerializeField] private Button femaleButton;

        [Header("Categories")]
        [SerializeField] private CategoryRow[] categories;
        [SerializeField] private Button facialHairButton;

        [Header("Control Buttons")]
        [SerializeField] private Button randomizeButton;

        private CategoryRow _prevCategory;

        public override void Initialize()
        {
            maleButton.onClick.AddListener(() => SelectGender(BodyType.Male));
            femaleButton.onClick.AddListener(() => SelectGender(BodyType.Female));

            foreach (var category in categories)
            {
                category.Button.onClick.AddListener(() => SelectCategory(category));
            }

            randomizeButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
                MsgBroker.Instance.Publish(new RandomizeCharacter());
            });

            base.Initialize();
        }

        protected override void BeforeShow(object ctx = null)
        {
            var dontReset = ctx.ValueByKey<bool>("dont_reset");

            if (!dontReset)
            {
                SelectGender(BodyType.Male, true);
                SelectCategory(categories[0]);    
            }
            
            base.BeforeShow(ctx);
        }

        protected override void AfterHide()
        {
            mainCamera.fieldOfView = 85;
            Character.Instance.SetPosition();

            base.AfterHide();
        }

        private void SelectGender(BodyType type, bool silent = false)
        {
            if (!silent)
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
            }

            if (type == BodyType.Male)
            {
                maleButton.image.sprite   = imagesBank.MaleAvatar;
                femaleButton.image.sprite = imagesBank.FemaleAvatarInactive;
                facialHairButton.gameObject.SetActive(true);
            } else
            {
                maleButton.image.sprite   = imagesBank.MaleAvatarInactive;
                femaleButton.image.sprite = imagesBank.FemaleAvatar;
                facialHairButton.gameObject.SetActive(false);
            }

            Character.Instance.Viewer.SetBodyType(type);
            MsgBroker.Instance.Publish(new ResetCharacter());
        }

        private void SelectCategory(CategoryRow category)
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_prevCategory != null)
            {
                _prevCategory.Category.SetActive(false);
                _prevCategory.Outline.SetActive(false);
                
                if (_prevCategory.CategoryColors != null)
                {
                    _prevCategory.CategoryColors.SetActive(false);
                }
            }

            category.Category.SetActive(true);
            category.Outline.SetActive(true);

            if (category.CategoryColors != null)
            {
                category.CategoryColors.SetActive(true);
            }

            if (category.ZoomCharacter)
            {
                mainCamera.fieldOfView = 40;
                Character.Instance.SetPosition(0.3f, -10f);
            } else
            {
                mainCamera.fieldOfView = 75;
                Character.Instance.SetPosition();
            }

            _prevCategory = category;
        }
    }
}