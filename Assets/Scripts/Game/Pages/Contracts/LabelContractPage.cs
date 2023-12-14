using System;
using Core;
using Data;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Contracts
{
    public class LabelContractPage : Page
    {
        [SerializeField] private Image logo;
        [SerializeField] private Text labelName;
        [SerializeField] private Text greeting;
        [SerializeField] private Text contract;
        [Space]
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button signButton;

        private void Start()
        {
            rejectButton.onClick.AddListener(OnReject);
            signButton.onClick.AddListener(OnSign);
        }

        public void Show(LabelInfo label)
        {
            logo.sprite = label.Logo;
            labelName.text = label.Name;
            // todo:
            // greeting.text = "";
            // contract.text = "";
            
            Open();
        }

        private void OnReject()
        {
            SoundManager.Instance.PlayClick();
            Close();
        }
        
        private void OnSign()
        {
            SoundManager.Instance.PlayClick();
            PlayerManager.Data.Label = labelName.text;
            Close();
        }
    }
}