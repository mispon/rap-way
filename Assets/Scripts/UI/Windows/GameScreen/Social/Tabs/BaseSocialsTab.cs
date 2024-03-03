using System;
using Game;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public abstract class BaseSocialsTab : MonoBehaviour
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite cooldownSprite;
        [SerializeField] private Button startButton;

        public event Action<SocialInfo> onStartSocial = info => {};

        private void Start()
        {
            startButton.onClick.AddListener(StartSocial);
            TabStart();
        }

        public void SetVisible(bool state)
        {
            if (state)
                OnOpen();

            gameObject.SetActive(state);
        }

        public void Refresh()
        {
            OnOpen();
        }

        private void StartSocial()
        {
            var info = GetInfo();
            onStartSocial.Invoke(info);
        }

        protected virtual void OnOpen()
        {
            if (CheckStartConditions())
            {
                startButton.interactable = true;
                startButton.image.sprite = normalSprite;
            } else
            {
                startButton.interactable = false;
                startButton.image.sprite = cooldownSprite;
            }
        }

        protected virtual bool CheckStartConditions()
        {
            return GameManager.Instance.GameStats.SocialsCooldown == 0;
        }

        protected virtual void TabStart() {}

        protected abstract SocialInfo GetInfo();
    }
}