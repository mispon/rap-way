using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.UnreadBadge
{
    public abstract class BaseUnreadBadge : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI value;

        private int counter;
        protected readonly CompositeDisposable disposables = new();

        private void Start()
        {
            HideBadge();
            RegisterHandlers();
        }

        protected abstract void RegisterHandlers();

        protected void IncCounter()
        {
            image.enabled = true;
            value.enabled = true;
            value.text = $"{++counter}";
        }

        protected void ResetCounter()
        {
            counter = 0;
            value.text = $"{counter}";
        }

        protected void HideBadge()
        {
            image.enabled = false;
            value.enabled = false;
        }

        private void OnDestroy()
        {
            disposables.Clear();    
        }
    }
}