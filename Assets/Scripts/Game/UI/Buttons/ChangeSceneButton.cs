using Core;
using Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    [RequireComponent(typeof(Button))]
    public sealed class ChangeSceneButton: MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SoundManager.Instance.PlaySound(_soundType);
                    
                    UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneName);
                })
                .AddTo(this);
        }
    }
}
