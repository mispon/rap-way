using System;
using Core;
using Data;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Labels
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

            float prestige = LabelsManager.Instance.GetLabelPrestige(info);
            stars.Display(prestige);
            
            deleteButton.gameObject.SetActive(info.IsCustom);
            openInfoButton.gameObject.SetActive(!info.IsPlayer);
        }

        private void OpenInfoView()
        {
            infoDesc.text = LocalizationManager.Instance.Get(_info.Desc);
            infoView.SetActive(true);
        }
        
        private void CloseInfoView()
        {
            infoView.SetActive(false);
        }
        
        private void DeleteLabel()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            onDelete.Invoke(_info);
        }
    }
}