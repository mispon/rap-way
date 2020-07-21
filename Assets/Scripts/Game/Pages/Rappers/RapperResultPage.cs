using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Станица результата переговоров
    /// </summary>
    public class RapperResultPage : Page
    {
        [SerializeField] private Text header;
        [Space]
        [SerializeField] private Button okButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button nextButton;

        private bool _isFeat;

        private void Start()
        {
            okButton.onClick.AddListener(Close);
            cancelButton.onClick.AddListener(Close);
            nextButton.onClick.AddListener(OnNext);
        }

        /// <summary>
        /// Открывает страницу с результатами переговоров
        /// </summary>
        public void Show(RapperInfo rapper, int workPoints, bool isFeat)
        {
            _isFeat = isFeat;
            
            bool result = AnalyzeConversations(rapper, workPoints);
            DisplayResult(result, rapper.Name);
            
            Open();
        }

        /// <summary>
        /// Анализирует успешность переговоров 
        /// </summary>
        private bool AnalyzeConversations(RapperInfo rapper, int workPoints)
        {
            // TODO: analyze result
            
            return true;
        }

        /// <summary>
        /// Показывает результаты
        /// </summary>
        private void DisplayResult(bool result, string rapperName)
        {
            string template = result ? "Успешные переговоры с" : "Неудачные переговоры с";
            header.text = $"{template} {rapperName}!";
            
            okButton.gameObject.SetActive(!result);
            cancelButton.gameObject.SetActive(result);
            nextButton.gameObject.SetActive(result);
        }

        /// <summary>
        /// Обработчик кнопки продолжения
        /// </summary>
        private void OnNext()
        {
            if (_isFeat)
            {
                // TODO: do feat
            }
            else
            {
                // TODO: do battle
            }
        }
    }
}