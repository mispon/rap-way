using CharacterCreator2D;
using CharacterCreator2D.UI;
using MessageBroker;
using MessageBroker.Messages.Player;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Windows.MainMenu.NewGame
{
    public class BodySegmentSlider : MonoBehaviour
    {
        [SerializeField] private CharacterViewer  character;
        [SerializeField] private InputFieldSlider input;
        [SerializeField] private SegmentType      segmentType;
        [SerializeField] private BodySliderType   sliderType;

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<RandomizeCharacter>()
                .Subscribe(m => { RandomizeScale(); })
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<ResetCharacter>()
                .Subscribe(m => { ResetScale(); })
                .AddTo(_disposable);

            input.onValueChanged.AddListener(UpdateScale);
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            input.slider.value = sliderType switch
            {
                BodySliderType.X           => character.GetBodySlider(segmentType).x,
                BodySliderType.Y           => character.GetBodySlider(segmentType).y,
                BodySliderType.Symmetrical => character.GetBodySlider(segmentType).x,
                _                          => input.slider.value
            };
        }

        private void UpdateScale(float value)
        {
            switch (sliderType)
            {
                case BodySliderType.X:
                    character.SetBodySlider(segmentType, new Vector2(value, character.GetBodySlider(segmentType).y));
                    break;
                case BodySliderType.Y:
                    character.SetBodySlider(segmentType, new Vector2(character.GetBodySlider(segmentType).x, value));
                    break;
                case BodySliderType.Symmetrical:
                    character.SetBodySlider(segmentType, new Vector2(value, value));
                    break;
            }
        }

        private void ResetScale()
        {
            switch (sliderType)
            {
                case BodySliderType.X:
                    character.SetBodySlider(segmentType, new Vector2(0.5f, character.GetBodySlider(segmentType).y));
                    break;
                case BodySliderType.Y:
                    character.SetBodySlider(segmentType, new Vector2(character.GetBodySlider(segmentType).x, 0.5f));
                    break;
                case BodySliderType.Symmetrical:
                    character.SetBodySlider(segmentType, new Vector2(0.5f, 0.5f));
                    break;
            }
        }

        private void RandomizeScale()
        {
            var extremeDice = Random.Range(0f, 1f);

            var value = extremeDice switch
            {
                > 0.85f => Random.Range(0.0f, 1.0f),
                > 0.5f  => Random.Range(0.3f, 0.7f),
                > 0.15f => Random.Range(0.4f, 0.6f),
                _       => 0.5f
            };

            switch (sliderType)
            {
                case BodySliderType.X:
                    character.SetBodySlider(segmentType, new Vector2(value, character.GetBodySlider(segmentType).y));
                    break;
                case BodySliderType.Y:
                    character.SetBodySlider(segmentType, new Vector2(character.GetBodySlider(segmentType).x, value));
                    break;
                case BodySliderType.Symmetrical:
                    character.SetBodySlider(segmentType, new Vector2(value, value));
                    break;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}