using Data;
using UnityEngine;

namespace Game.Pages.Store
{
    /// <summary>
    /// Страница магазина
    /// Инициализирует шмокти следующего уровня в двух ScrollView (шмотки для работы и понты)
    /// </summary>
    public class StorePage: Page
    {
        [Header("Контроллеры управления UI-элементов")]
        [SerializeField] private ScrollItemsController workToolsScrollItemsController;
        [SerializeField] private ScrollItemsController swagScrollItemsController;
        
        [Header("Данные")]
        [SerializeField] private GoodsData data;

        protected override void BeforePageOpen()
        {
            workToolsScrollItemsController.Initialize(data.WorkTools);
            swagScrollItemsController.Initialize(data.Swag);
        }

        protected override void AfterPageClose()
        {
            workToolsScrollItemsController.Dispose();
            swagScrollItemsController.Dispose();
        }
    }
}