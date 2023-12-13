using System;
using Core;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Labels
{
    public class LabelCard : MonoBehaviour
    {
        [SerializeField] private Text labelName;
        [SerializeField] private Text production;
        [SerializeField] private Text score;
        [SerializeField] private Image logo;
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
            score.text = $"{info.Score}";
            // todo: display prestige stars
            
            deleteButton.gameObject.SetActive(info.IsCustom);
        }

        private void DeleteLabel()
        {
            SoundManager.Instance.PlayClick();
            onDelete.Invoke(_info);
        }
    }
}