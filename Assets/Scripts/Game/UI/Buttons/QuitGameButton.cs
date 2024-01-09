using Core;
using Data;
using Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class QuitGameButton : MonoBehaviour
    {
        [SerializeField] private UIActionType _soundType = UIActionType.Click;

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            gameObject.SetActive(false);
#else
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SoundManager.Instance.PlaySound(_soundType);

                    GameManager.Instance.SaveApplicationData();
                    
                    Application.Quit();
                });
#endif
        }
    }
}