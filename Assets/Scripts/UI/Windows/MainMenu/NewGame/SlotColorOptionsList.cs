using System.Collections.Generic;
using CharacterCreator2D;
using MessageBroker;
using MessageBroker.Messages.Player;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;

namespace UI.Windows.MainMenu.NewGame
{
    public class SlotColorOptionsList : MonoBehaviour
    {
        [SerializeField] private CharacterViewer      character;
        [SerializeField] private ScrollViewController scrollView;
        [SerializeField] private GameObject           optionTemplate;
        [SerializeField] private Color[]              colors;

        private readonly List<SlotColorOption> _options    = new();
        private readonly CompositeDisposable   _disposable = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<RandomizeCharacter>()
                .Subscribe(m =>
                {
                    var color = colors[Random.Range(0, colors.Length)];
                    _options[0].SetPartColor(color);
                })
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<ResetCharacter>()
                .Subscribe(m => { _options[0].SetPartColor(colors[0]); })
                .AddTo(_disposable);

            for (var i = 0; i < colors.Length; i++)
            {
                var option = scrollView.InstantiatedElement<SlotColorOption>(optionTemplate);
                option.Initialize(i, character, colors[i]);

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