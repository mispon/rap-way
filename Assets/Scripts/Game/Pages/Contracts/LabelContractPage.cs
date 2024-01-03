using Core;
using Data;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Contracts
{
    public class LabelContractPage : Page
    {
        [SerializeField] private Image logo;
        [SerializeField] private Text greeting;
        [SerializeField] private Text contract;
        [Space]
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button signButton;

        private string _labelName;
        
        private void Start()
        {
            rejectButton.onClick.AddListener(OnReject);
            signButton.onClick.AddListener(OnSign);
        }

        public void Show(LabelInfo label)
        {
            if (PlayerManager.Data.Label != "")
                return;
            
            string playerNickname = PlayerManager.Data.Info.NickName;
            _labelName = label.Name;
            
            logo.sprite = label.Logo;
            greeting.text = GetLocale("label_contract_greeting", _labelName);
            contract.text = GetLocale("label_contract_text", playerNickname, _labelName, _labelName);
            
            Open();
        }

        private void OnReject()
        {
            SoundManager.Instance.PlayClick();
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.LabelContractDeclined);
            Close();
        }
        
        private void OnSign()
        {
            SoundManager.Instance.PlayClick();
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.LabelContractAccepted);
            PlayerManager.Data.Label = _labelName;
            Close();
        }

        protected override void AfterPageClose()
        {
            _labelName = "";
        }
    }
}