using Core;
using Enums;
using Game.Player;
using Game.Rappers.Desc;
using ScriptableObjects;
using UI.Windows.Pages.Battle;
using UI.Windows.Pages.Charts;
using UI.Windows.Pages.Feat;
using UnityEngine;
using UnityEngine.UI;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.Pages.Rappers
{
    /// <summary>
    /// Станица результата переговоров
    /// </summary>
    public class RapperResultPage : Page
    {
        [Header("Контролы")]
        [SerializeField] private Text message;
        [SerializeField] private Image rapperAvatar;
        [SerializeField] private Sprite customRapperAvatar;
        [Space]
        [SerializeField] private Button okButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button nextButton;

        [Header("Страницы")]
        [SerializeField] private ChartsPage chartsPage;
        [SerializeField] private RappersPage rappersPage;
        [SerializeField] private FeatSettingsPage featPage;
        [SerializeField] private BattleWorkingPage battlePage;

        private RapperInfo _rapper;
        private ConversationType _convType;

        private void Start()
        {
            okButton.onClick.AddListener(OnClose);
            cancelButton.onClick.AddListener(OnClose);
            nextButton.onClick.AddListener(OnNext);
        }

        /// <summary>
        /// Открывает страницу с результатами переговоров
        /// </summary>
        public void Show(RapperInfo rapper, int playerPoints, int rapperPoints, ConversationType convType)
        {
            _rapper = rapper;
            _convType = convType;
            
            bool result = AnalyzeConversations(playerPoints, rapperPoints);
            DisplayResult(result, rapper.Name);
            
            Open();
        }

        /// <summary>
        /// Анализирует успешность переговоров 
        /// </summary>
        private static bool AnalyzeConversations(int playerPoints, int rapperPoints)
        {
            var hypeBonus = PlayerManager.Data.Hype / 5;
            return playerPoints + hypeBonus > rapperPoints;
        }

        /// <summary>
        /// Показывает результаты
        /// </summary>
        private void DisplayResult(bool result, string rapperName)
        {
            string key = result ? "conversations_success" : "conversations_fail";
            message.text = $"{GetLocale(key)} <color=#01C6B8>{rapperName}</color>!";
            rapperAvatar.sprite = _rapper.IsCustom ? customRapperAvatar : _rapper.Avatar;
            
            okButton.gameObject.SetActive(!result);
            cancelButton.gameObject.SetActive(result);
            nextButton.gameObject.SetActive(result);
        }

        /// <summary>
        /// Обработчик кнопки продолжения
        /// </summary>
        private void OnNext()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            switch (_convType)
            {
                case ConversationType.Feat:
                    featPage.Show(_rapper);
                    break;
                case ConversationType.Battle:
                    battlePage.StartWork(_rapper);
                    break;
                case ConversationType.Label:
                    _rapper.Label = PlayerManager.Data.Label;
                    LabelsAPI.Instance.RefreshScore(_rapper.Label);
                    break;
                default:
                    Debug.LogError($"Unexpected conv type {_convType.ToString()}");
                    break;
            }
            
            chartsPage.Close();
            Close();
        }

        protected override void AfterPageClose()
        {
            _rapper = null;
        }

        /// <summary>
        /// Обработчик кнопок отмены и возврата
        /// </summary>
        private void OnClose()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            rappersPage.Open();
            chartsPage.Show();
            
            Close();
        }
    }
}