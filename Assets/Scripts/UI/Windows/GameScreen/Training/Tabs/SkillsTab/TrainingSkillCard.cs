using System;
using Core;
using Enums;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.SkillsTab
{
    /// <summary>
    /// Карточка умения в гриде умений персонажа
    /// </summary>
    public class TrainingSkillCard : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TrainingSkillView view;

        public Action<Skills> onUnlock = skill => {};
        
        private PlayerSkillInfo _info;
        private bool _expEnough;
        private bool _locked;

        private void Start()
        {
            button.onClick.AddListener(ShowInfo);
        }

        public void Setup(int index, PlayerSkillInfo info)
        {
            _info = info;

            name = $"SkillCard-{index + 1}";
            gameObject.SetActive(true);
        }
        
        public void Refresh(bool expEnough)
        {
            _expEnough = expEnough;
            _locked = !PlayerAPI.Data.Skills.Contains(_info.Type);
            
            button.image.sprite = _locked ? _info.Locked : _info.Normal;
        }

        /// <summary>
        /// Показывает информацию об умении
        /// </summary>
        private void ShowInfo()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            view.Show(_info, _expEnough, _locked, onUnlock);
        }
    }
}