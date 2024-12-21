using CharacterCreator2D;
using Core;
using MessageBroker;
using MessageBroker.Messages.Player;
using ScriptableObjects;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu.NewGame
{
    public class CharacterCreatorWindow : CanvasUIElement
    {
        [SerializeField] private CharacterViewer character;
        [SerializeField] private ImagesBank      imagesBank;

        [Header("Gender")]
        [SerializeField] private Button maleButton;
        [SerializeField] private Button femaleButton;

        [Header("Categories Buttons")]
        [SerializeField] private Button commonButton;
        [SerializeField] private Button hairButton;
        [SerializeField] private Button facialHairButton;
        [SerializeField] private Button eyesButton;
        [SerializeField] private Button eyebrowButton;
        [SerializeField] private Button noseButton;
        [SerializeField] private Button mouthButton;

        [Header("Categories")]
        [SerializeField] private GameObject common;
        [SerializeField] private GameObject hairs;
        [SerializeField] private GameObject facialHairs;
        [SerializeField] private GameObject eyes;
        [SerializeField] private GameObject eyebrows;
        [SerializeField] private GameObject noses;
        [SerializeField] private GameObject mouths;

        [Header("Category Colors")]
        [SerializeField] private GameObject bodyColors;
        [SerializeField] private GameObject hairColors;
        [SerializeField] private GameObject facialHairColors;
        [SerializeField] private GameObject eyesColors;
        [SerializeField] private GameObject eyebrowsColors;
        [SerializeField] private GameObject mouthsColors;

        [Header("Control Buttons")]
        [SerializeField] private Button randomizeButton;
        [SerializeField] private Button nextButton;

        [Header("Colors")]
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        private GameObject _prevCategory;
        private GameObject _prevCategoryColors;

        public override void Initialize()
        {
            maleButton.onClick.AddListener(() => SelectGender(BodyType.Male));
            femaleButton.onClick.AddListener(() => SelectGender(BodyType.Female));

            commonButton.onClick.AddListener(() => SelectCategory(common, bodyColors));
            hairButton.onClick.AddListener(() => SelectCategory(hairs, hairColors));
            facialHairButton.onClick.AddListener(() => SelectCategory(facialHairs, facialHairColors));
            eyesButton.onClick.AddListener(() => SelectCategory(eyes, eyesColors));
            eyebrowButton.onClick.AddListener(() => SelectCategory(eyebrows, eyebrowsColors));
            noseButton.onClick.AddListener(() => SelectCategory(noses));
            mouthButton.onClick.AddListener(() => SelectCategory(mouths, mouthsColors));

            randomizeButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
                MsgBroker.Instance.Publish(new RandomizeCharacter());
            });
            nextButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
                // click
            });

            SelectCategory(common, bodyColors);
            base.Initialize();
        }

        private void SelectGender(BodyType type)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

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

            character.SetBodyType(type);
            character.ResetEmote();

            MsgBroker.Instance.Publish(new ResetCharacter());
        }

        private void SelectCategory(GameObject category, GameObject categoryColors = null)
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_prevCategory != null)
            {
                _prevCategory.SetActive(false);
            }

            if (_prevCategoryColors != null)
            {
                _prevCategoryColors.SetActive(false);
            }

            category.SetActive(true);
            _prevCategory = category;

            categoryColors?.SetActive(true);
            _prevCategoryColors = categoryColors;
        }
    }
}