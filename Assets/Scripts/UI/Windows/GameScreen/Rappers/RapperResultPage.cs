using Core;
using Core.Context;
using Enums;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Enums;
using UI.Windows.GameScreen.Charts;
using UnityEngine;
using UnityEngine.UI;
using LabelsAPI = Game.Labels.LabelsPackage;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Rappers
{
    public class RapperResultPage : Page
    {
        [BoxGroup("Result"), SerializeField] private Text message;
        [BoxGroup("Result"), SerializeField] private Image rapperAvatar;
        [BoxGroup("Result"), SerializeField] private Sprite customRapperAvatar;
        [BoxGroup("Result"), SerializeField] private Button okButton;
        [BoxGroup("Result"), SerializeField] private Button cancelButton;
        [BoxGroup("Result"), SerializeField] private Button nextButton;

        private RapperInfo _rapper;
        private ConversationType _convType;

        private void Start()
        {
            okButton.onClick.AddListener(OnClose);
            cancelButton.onClick.AddListener(OnClose);
            nextButton.onClick.AddListener(OnNext);
        }
        
        public override void Show(object ctx = null)
        {
            _rapper   = ctx.ValueByKey<RapperInfo>("rapper");
            _convType = ctx.ValueByKey<ConversationType>("conv_type");
            
            var playerPoints = ctx.ValueByKey<int>("player_points");
            var rapperPoints = ctx.ValueByKey<int>("rapper_points");
            
            bool result = AnalyzeConversations(playerPoints, rapperPoints);
            DisplayResult(result, _rapper.Name);
            
            base.Show(ctx);
        }

        private static bool AnalyzeConversations(int playerPoints, int rapperPoints)
        {
            var hypeBonus = PlayerAPI.Data.Hype / 5;
            return playerPoints + hypeBonus > rapperPoints;
        }

        private void DisplayResult(bool result, string rapperName)
        {
            string key = result ? "conversations_success" : "conversations_fail";
            message.text = $"{GetLocale(key)} <color=#01C6B8>{rapperName}</color>!";
            rapperAvatar.sprite = _rapper.IsCustom ? customRapperAvatar : _rapper.Avatar;
            
            okButton.gameObject.SetActive(!result);
            cancelButton.gameObject.SetActive(result);
            nextButton.gameObject.SetActive(result);
        }

        private void OnNext()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            switch (_convType)
            {
                case ConversationType.Feat:
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Type = WindowType.ProductionFeatSettings,
                        Context = _rapper
                    });
                    break;
                
                case ConversationType.Battle:
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Type = WindowType.BattleWork,
                        Context = _rapper
                    });
                    break;

                case ConversationType.Label:
                    _rapper.Label = PlayerAPI.Data.Label;
                    LabelsAPI.Instance.RefreshScore(_rapper.Label);
                    MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.GameScreen));
                    break;

                default:
                    MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.GameScreen));
                    Debug.LogError($"Unexpected conv type {_convType.ToString()}");
                    break;
            }
        }

        protected override void AfterHide()
        {
            _rapper = null;
        }

        private static void OnClose()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type    = WindowType.Charts,
                Context = ChartsTabType.Rappers
            });
        }
    }
}