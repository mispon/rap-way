using Core;
using Data;
using Game.Pages.Battle;
using Game.Pages.Feat;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Станица результата переговоров
    /// </summary>
    public class RapperResultPage : Page
    {
        [Header("Контролы")]
        [SerializeField] private Text message;
        [SerializeField] private Image rapperAvatar;
        [Space]
        [SerializeField] private Button okButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button nextButton;

        [Header("Страницы")]
        [SerializeField] private FeatSettingsPage featPage;
        [SerializeField] private BattleWorkingPage battlePage;

        private RapperInfo _rapper;
        private bool _isFeat;

        private void Start()
        {
            okButton.onClick.AddListener(OnClose);
            cancelButton.onClick.AddListener(OnClose);
            nextButton.onClick.AddListener(OnNext);
        }

        /// <summary>
        /// Открывает страницу с результатами переговоров
        /// </summary>
        public void Show(RapperInfo rapper, int playerPoints, int rapperPoints, bool isFeat)
        {
            _rapper = rapper;
            _isFeat = isFeat;
            
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
            rapperAvatar.sprite = _rapper.Avatar;
            
            okButton.gameObject.SetActive(!result);
            cancelButton.gameObject.SetActive(result);
            nextButton.gameObject.SetActive(result);
        }

        /// <summary>
        /// Обработчик кнопки продолжения
        /// </summary>
        private void OnNext()
        {
            SoundManager.Instance.PlayClick();
            
            if (_isFeat)
                featPage.Show(_rapper);
            else
                battlePage.StartWork(_rapper);
            
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
            SoundManager.Instance.PlayClick();
            Close();
        }
    }
}