using Enums;
using Game.Labels.Desc;
using Game.Settings;
using UnityEngine;

namespace Game.Labels.AI
{
    public partial class LabelsAI
    {
        public void DoAction(LabelInfo labelInfo, GameSettings settings)
        {
            var action = ChooseAction();

            switch (action)
            {
                case LabelsAIActions.None:
                    // do nothing
                    break;

                case LabelsAIActions.InvitePlayer:
                    DoInvitePlayer(labelInfo, settings);
                    break;

                case LabelsAIActions.InviteRapper:
                    DoInviteRapper(labelInfo, settings);
                    break;

                default:
                    Debug.LogError($"Received unexpected action type: {action}");
                    break;
            }
        }

        private static LabelsAIActions ChooseAction()
        {
            return RollDice() switch
            {
                < 20 => LabelsAIActions.InvitePlayer,
                < 50 => LabelsAIActions.InviteRapper,
                _    => LabelsAIActions.None
            };
        }

        private static int RollDice()
        {
            return Random.Range(0, 100);
        }
    }
}