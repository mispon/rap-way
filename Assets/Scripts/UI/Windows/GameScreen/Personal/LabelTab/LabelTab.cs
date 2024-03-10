using UnityEngine;
using LabelsAPI = Game.Labels.LabelsPackage;
using PlayerAPI = Game.Player.PlayerPackage;

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
            
            string labelName = PlayerAPI.Data.Label;
            if (string.IsNullOrEmpty(labelName))
            {
                noLabelTab.Open();
                return;
            }

            var label = LabelsAPI.Instance.Get(labelName);
            if (label == null)
            {
                PlayerAPI.Data.Label = "";
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