using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Personal.LabelTab
{
    public class NoLabelSubTab : Tab
    {
        [SerializeField] private LabelTab labelTab;
        [Space]
        [SerializeField] private InputField labelNameInput;
        [SerializeField] private Button createButton;

        private void Start()
        {
            createButton.onClick.AddListener(CreateLabel);
        }

        private void CreateLabel()
        {
            string labelName = labelNameInput.text;

            // todo:
            
            PlayerManager.Data.Label = labelName;
            labelTab.Reload();
        }
    }
}