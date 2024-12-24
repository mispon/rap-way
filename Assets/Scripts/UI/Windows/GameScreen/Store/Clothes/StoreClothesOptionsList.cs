using System.Collections.Generic;
using CharacterCreator2D;
using Enums;
using Game.Player;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;

namespace UI.Windows.GameScreen.Store.Clothes
{
    public class StoreClothesOptionsList : MonoBehaviour
    {
        [SerializeField] private ScrollViewController scrollView;
        [SerializeField] private GameObject           optionTemplate;

        [SerializeField] private Part[] parts;
        [SerializeField] private Part[] partsMale;
        [SerializeField] private Part[] partsFemale;

        private readonly List<StoreClothesOption> _options    = new();
        private readonly CompositeDisposable      _disposable = new();

        private void Start()
        {
            var items = parts.Length > 0
                ? parts
                : PlayerPackage.Data.Info.Gender == Gender.Male
                    ? partsMale
                    : partsFemale;

            for (var i = 0; i < items.Length; i++)
            {
                var option = scrollView.InstantiatedElement<StoreClothesOption>(optionTemplate);
                option.Initialize(i, items[i]);

                _options.Add(option);
            }

            scrollView.RepositionElements(_options);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            foreach (var option in _options)
            {
                Destroy(option.gameObject);
            }

            _options.Clear();
            _disposable.Clear();
        }
    }
}