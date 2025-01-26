using System.Collections;
using System.Collections.Generic;
using CharacterCreator2D;
using Game;
using MessageBroker;
using MessageBroker.Messages.Player;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;

namespace UI.CharacterCreator
{
    public class SlotOptionsList : MonoBehaviour
    {
        [SerializeField] private ScrollViewController scrollView;
        [SerializeField] private GameObject           optionTemplate;
        [SerializeField] private Part[]               parts;
        [SerializeField] private int                  defaultPart;

        private readonly List<SlotOption>    _options    = new();
        private readonly CompositeDisposable _disposable = new();

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance.Ready);

            MsgBroker.Instance
                .Receive<RandomizeCharacter>()
                .Subscribe(m =>
                {
                    var idx = Random.Range(0, _options.Count);
                    _options[idx].SelectPart();
                })
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<ResetCharacter>()
                .Subscribe(m => { _options[defaultPart].SelectPart(); })
                .AddTo(_disposable);

            for (var i = 0; i < parts.Length; i++)
            {
                var option = scrollView.InstantiatedElement<SlotOption>(optionTemplate);
                option.Initialize(i, parts[i], BeforeClickCallback);

                _options.Add(option);
            }

            scrollView.RepositionElements(_options);
            gameObject.SetActive(false);
        }

        private void BeforeClickCallback()
        {
            foreach (var option in _options)
            {
                option.HideOutline();
            }
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