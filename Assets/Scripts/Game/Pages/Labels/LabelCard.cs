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
        [SerializeField] private Button deleteButton;
        
        public event Action<LabelInfo> onDelete = _ => {};

        private LabelInfo _info;
        
        private void Start()
        {
            deleteButton.onClick.AddListener(DeleteLabel);
        }
        
        public void Show(LabelInfo info)
        {
            _info = info;
            
            labelName.text = info.Name.ToUpper();
            logo.sprite = info.Logo;
            production.text = $"{info.Production.Value}";

            string scoreText = LocalizationManager.Instance.GetFormat("score_label", info.Score);
            score.text = scoreText.ToUpper();

            float prestige = LabelsManager.Instance.GetLabelPrestige(info);
            stars.Display(prestige);
            
            deleteButton.gameObject.SetActive(info.IsCustom);
        }

        private void DeleteLabel()
        {
            SoundManager.Instance.PlayClick();
            onDelete.Invoke(_info);
        }
    }
}