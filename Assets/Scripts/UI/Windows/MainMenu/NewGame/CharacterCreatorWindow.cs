using CharacterCreator2D;
using Core;
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

        public override void Initialize()
        {
            maleButton.onClick.AddListener(() => SelectGender(BodyType.Male));
            femaleButton.onClick.AddListener(() => SelectGender(BodyType.Female));

            base.Initialize();
        }

        private void SelectGender(BodyType type)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            if (type == BodyType.Male)
            {
                maleButton.image.sprite   = imagesBank.MaleAvatar;
                femaleButton.image.sprite = imagesBank.FemaleAvatarInactive;
            } else
            {
                maleButton.image.sprite   = imagesBank.MaleAvatarInactive;
                femaleButton.image.sprite = imagesBank.FemaleAvatar;
            }

            character.SetBodyType(type);
            character.ResetEmote();
        }
    }
}