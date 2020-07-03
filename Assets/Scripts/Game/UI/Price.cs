using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Отображение цены в интерфейсе
    /// </summary>
    public class Price : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private Animation errorAnim;

        /// <summary>
        /// Устанавливает значение цены 
        /// </summary>
        public void SetValue(string value)
        {
            label.text = value;
        }
        
        /// <summary>
        /// Проигрывает эффект нехватки денег
        /// </summary>
        public void ShowNoMoney()
        {
            errorAnim.Play();
        }
    }
}