using Cysharp.Threading.Tasks;
using RapWay.Core.UI;
using RapWay.UI.HUD;
using VContainer.Unity;

namespace RapWay.App
{
    public class GameStartup : IStartable
    {
        private readonly UIService _uiService;

        public GameStartup(UIService uiService)
        {
            _uiService = uiService;
        }

        public void Start()
        {
            _uiService.Open<HUDWindow>().Forget();
        }
    }
}