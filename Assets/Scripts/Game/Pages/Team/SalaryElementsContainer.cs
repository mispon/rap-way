using Localization;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Pages.Team
{
    /// <summary>
    /// Элемент зарплаты тиммейта
    /// </summary>
    public class SalaryElementsContainer: MonoBehaviour
    {
        [SerializeField] private Text typeText;
        [SerializeField] private Text salaryText;
        [SerializeField] private Button payButton;

        private Teammate _teammate;
        private int _salary;

        private void Start()
        {
            payButton.onClick.AddListener(Pay);
        }

        /// <summary>
        /// Активация элемента
        /// </summary>
        public void Setup(Teammate teammate, int salary)
        {
            _teammate = teammate;
            _salary = salary;
         
            typeText.text = LocalizationManager.Instance.Get(_teammate.Type.GetDescription());
            salaryText.text = $"{_salary} $";
            
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Сброс настроек элемента
        /// </summary>
        public void Reset()
        {
            _teammate = null;
            _salary = 0;

            typeText.text = "";
            salaryText.text = "";

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Попытка оплатиты
        /// </summary>
        private void Pay()
        {
            if (PlayerManager.Instance.Pay(_salary))
            {
                _teammate.HasPayment = true;
                Reset();
            }
            else
            {
                // todo: use Price prefab
                Debug.Log($"Недостаточно денег: {GameManager.Instance.PlayerData.Data.GetMoney()}");
            }
        }
    }
}