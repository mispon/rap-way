using System.Collections.Generic;
using CharacterCreator2D;
using MessageBroker;
using MessageBroker.Messages.Player;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;

namespace UI.Windows.MainMenu.NewGame
{
    public class SlotOptionsList : MonoBehaviour
    {
        [SerializeField] private CharacterViewer      character;
        [SerializeField] private ScrollViewController scrollView;
        [SerializeField] private GameObject           optionTemplate;
        [SerializeField] private Part[]               parts;

        private readonly List<SlotOption>    _options    = new();
        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<RandomizeCharacter>()
                .Subscribe(m =>
                {
                    var part = parts[Random.Range(0, parts.Length)];
                    _options[0].RandomizePart(part);
                })
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<ResetCharacter>()
                .Subscribe(m => { _options[0].ResetPart(parts[0]); })
                .AddTo(_disposable);

            for (var i = 0; i < parts.Length; i++)
            {
                var option = scrollView.InstantiatedElement<SlotOption>(optionTemplate);
                option.Initialize(i, character, parts[i]);

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