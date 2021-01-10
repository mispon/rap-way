using Data;
using Game.Effects;
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
        [SerializeField] private ScrollItemsController swagScrollItemsController;
        [SerializeField] private ScrollItemsController workScrollItemsController;
        
        [Header("Эффект открытия новой шмотки")]
        [SerializeField] private NewItemEffect newGoodEffect;
        
        [Header("Данные")]
        [SerializeField] private GoodsData data;

        protected override void BeforePageOpen()
        {
            workScrollItemsController.Initialize(data.WorkTools, newGoodEffect);
            swagScrollItemsController.Initialize(data.Swag, newGoodEffect);
        }

        protected override void AfterPageClose()
        {
            workScrollItemsController.Dispose();
            swagScrollItemsController.Dispose();
        }
    }
}