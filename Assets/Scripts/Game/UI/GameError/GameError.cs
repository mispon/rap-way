using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GameError
{
    public class GameError : MonoBehaviour
    {
        [SerializeField] private Text error;

        public void Show(string errorText)
        {
            error.text = errorText;
            gameObject.SetActive(true);
            
            StartCoroutine(Hide());
        }

        private IEnumerator Hide()
        {
            yield return new WaitForSeconds(3);
            gameObject.SetActive(false);
        }
    }
}