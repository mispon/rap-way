using System.Collections.Generic;
using CharacterCreator2D;
using UI.Controls.ScrollViewController;
using UnityEngine;

namespace UI.Windows.MainMenu.NewGame
{
    public class SlotOptionsList : MonoBehaviour
    {
        [SerializeField] private CharacterViewer character;

        [SerializeField] private ScrollViewController scrollView;
        [SerializeField] private GameObject           optionTemplate;

        [SerializeField] private Part[] parts;

        private readonly List<SlotOption> _options = new();

        private void Start()
        {
            for (var i = 0; i < parts.Length; i++)
            {
                var option = scrollView.InstantiatedElement<SlotOption>(optionTemplate);
                option.Initialize(i, character, parts[i]);

                _options.Add(option);
            }

            scrollView.RepositionElements(_options);
        }

        private void OnDestroy()
        {
            foreach (var option in _options)
            {
                Destroy(option.gameObject);
            }

            _options.Clear();
        }
    }
}