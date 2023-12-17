using UnityEngine;

namespace Game.Pages.Personal.LabelTab
{
    public class LabelTab : Tab
    {
        [SerializeField] private NoLabelSubTab noLabelTab;
        [SerializeField] private GameLabelSubTab gameLabelTab;
        [SerializeField] private PlayersLabelSubTab playersLabelTab;
        
        public override void Open()
        {
            string labelName = PlayerManager.Data.Label;
            if (labelName != "")
            {
                var label = LabelsManager.Instance.GetLabel(labelName);
                if (label.IsPlayer)
                    playersLabelTab.Show(label);
                else
                    gameLabelTab.Show(label);
            } else
            {
                noLabelTab.Open();
            }
            
            base.Open();
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