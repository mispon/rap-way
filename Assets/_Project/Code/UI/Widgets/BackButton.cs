using Cysharp.Threading.Tasks;
using RapWay.Core.UI;
using RapWay.UI.Windows;
using UnityEngine;
using VContainer;

namespace RapWay.UI.Widgets
{
    public class BackButton : BaseButton
    {
        private UIService _uiService;

        [Inject]
        public void Construct(UIService uiService)
        {
            _uiService = uiService;
        }

        protected void Start()
        {
            AddListener(GoBack);
        }

        private void GoBack()
        {
            _uiService.Back().Forget();
        }
    }
}