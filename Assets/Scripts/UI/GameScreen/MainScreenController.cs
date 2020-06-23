using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    /// <summary>
    /// Контроллер главного окна игры
    /// </summary>
    public class MainScreenController: MonoBehaviour
    {
        [Header("Группа основных действий")]
        [SerializeField] private Animation foldoutAnimation;
        [SerializeField] private Button productionFoldoutButton;
        
        private bool _productionShown;
        
        private void Start()
        {
            productionFoldoutButton.onClick.AddListener(OnProductionClick);
        }

        private void OnProductionClick()
        {
            _productionShown = !_productionShown;
            foldoutAnimation.Play(_productionShown ? "ProductionShow" : "ProductionHide");
        }
    }
}