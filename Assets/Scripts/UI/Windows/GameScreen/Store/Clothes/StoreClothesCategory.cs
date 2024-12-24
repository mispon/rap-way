using Core;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store.Clothes
{
    public class StoreClothesCategory : MonoBehaviour
    {
        [Header("Category Buttons")]
        [SerializeField] private Button hatsButton;
        [SerializeField] private Button outwearButton;
        [SerializeField] private Button pantsButton;
        [SerializeField] private Button glovesButton;
        [SerializeField] private Button bootsButton;
        [SerializeField] private Button otherButton;

        [Header("Categories")]
        [SerializeField] private GameObject hats;
        [SerializeField] private GameObject outwear;
        [SerializeField] private GameObject pants;
        [SerializeField] private GameObject gloves;
        [SerializeField] private GameObject boots;
        [SerializeField] private GameObject other;

        private GameObject _prevCategory;

        private void Start()
        {
            hatsButton.onClick.AddListener(() => SelectCategory(hats));
            outwearButton.onClick.AddListener(() => SelectCategory(outwear));
            pantsButton.onClick.AddListener(() => SelectCategory(pants));
            glovesButton.onClick.AddListener(() => SelectCategory(gloves));
            bootsButton.onClick.AddListener(() => SelectCategory(boots));
            otherButton.onClick.AddListener(() => SelectCategory(other));

            SelectCategory(hats);
        }

        private void SelectCategory(GameObject category)
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_prevCategory != null)
            {
                _prevCategory.SetActive(false);
            }

            category.SetActive(true);
            _prevCategory = category;
        }
    }
}