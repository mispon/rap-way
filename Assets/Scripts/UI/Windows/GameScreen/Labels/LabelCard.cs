﻿using System;
using Core;
using Core.Localization;
using Game.Labels.Desc;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Labels
{
    public class LabelCard : MonoBehaviour
    {
        [SerializeField] private Image logo;
        [SerializeField] private Text labelName;
        [SerializeField] private Text production;
        [SerializeField] private Text score;
        [SerializeField] private PrestigeStars stars;
        [Space]
        [SerializeField] private Sprite customLabelLogo;
        [SerializeField] private Button deleteButton;
        [Space]
        [SerializeField] private GameObject infoView;
        [SerializeField] private Text infoDesc;
        [SerializeField] private Button openInfoButton;
        [SerializeField] private Button closeInfoButton;
        
        public event Action<LabelInfo> onDelete = _ => {};

        private LabelInfo _info;
        
        private void Start()
        {
            openInfoButton.onClick.AddListener(OpenInfoView);
            closeInfoButton.onClick.AddListener(CloseInfoView);
            deleteButton.onClick.AddListener(DeleteLabel);
        }
        
        public void Show(LabelInfo info)
        {
            CloseInfoView();
            
            _info = info;
            
            labelName.text = info.Name.ToUpper();
            logo.sprite = info.IsPlayer ? customLabelLogo : info.Logo;
            production.text = $"{info.Production.Value}";

            string scoreText = LocalizationManager.Instance.GetFormat("score_label", info.Score);
            score.text = scoreText.ToUpper();

            float prestige = LabelsAPI.Instance.GetPrestige(info);
            stars.Display(prestige);
            
            deleteButton.gameObject.SetActive(info.IsCustom);
            openInfoButton.gameObject.SetActive(!info.IsPlayer);
        }

        private void OpenInfoView()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            infoDesc.text = LocalizationManager.Instance.Get(_info.Desc);
            infoView.SetActive(true);
        }
        
        private void CloseInfoView()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            infoView.SetActive(false);
        }
        
        private void DeleteLabel()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            onDelete.Invoke(_info);
        }
    }
}