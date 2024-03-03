using Game.Player;
using UI.Windows.Pages;
using UnityEngine;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Personal.LabelTab
{
    public class LabelTab : Tab
    {
        [SerializeField] private NoLabelSubTab noLabelTab;
        [SerializeField] private GameLabelSubTab gameLabelTab;
        [SerializeField] private PlayersLabelSubTab playersLabelTab;
        
        public override void Open()
        {
            base.Open();
            
            string labelName = PlayerManager.Data.Label;
            if (string.IsNullOrEmpty(labelName))
            {
                noLabelTab.Open();
                return;
            }

            var label = LabelsAPI.Instance.Get(labelName);
            if (label == null)
            {
                PlayerManager.Data.Label = "";
                noLabelTab.Open();
                return;
            }
                
            if (label.IsPlayer)
                playersLabelTab.Show(label);
            else
                gameLabelTab.Show(label);
        }

        public override void Close()
        {
            noLabelTab.Close();
            gameLabelTab.Close();
            playersLabelTab.Close();
            
            base.Close();
        }

        public void ShowMoneyReport()
        {
            playersLabelTab.ShowMoneyReport();
        }

        public void Reload()
        {
            Close();
            Open();
        }
    }
}