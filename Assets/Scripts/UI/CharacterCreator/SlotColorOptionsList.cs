using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using MessageBroker;
using MessageBroker.Messages.Player;
using UI.Controls.ScrollViewController;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.CharacterCreator
{
    [Serializable]
    public struct SlotColor
    {
        public Color Color1;
        public Color Color2;
        public Color Color3;
    }

    public class SlotColorOptionsList : MonoBehaviour
    {
        [SerializeField] private ScrollViewController scrollView;
        [SerializeField] private GameObject           optionTemplate;
        [SerializeField] private SlotColor[]          colors;
        [SerializeField] private int                  defaultColor;

        private readonly List<SlotColorOption> _options    = new();
        private readonly CompositeDisposable   _disposable = new();

        public void Show()
        {
            BeforeClickCallback();
            gameObject.SetActive(true);

            _options[0].SelectOption();
        }

        public void Hide()
        {
            BeforeClickCallback();
            gameObject.SetActive(false);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance.Ready);

            MsgBroker.Instance
                .Receive<RandomizeCharacter>()
                .Subscribe(m =>
                {
                    var idx = Random.Range(0, _options.Count);
                    _options[idx].SelectOption();
                })
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<ResetCharacter>()
                .Subscribe(m => { _options[defaultColor].SelectOption(); })
                .AddTo(_disposable);

            for (var i = 0; i < colors.Length; i++)
            {
                var option = scrollView.InstantiatedElement<SlotColorOption>(optionTemplate);
                option.Initialize(i, colors[i], BeforeClickCallback);

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