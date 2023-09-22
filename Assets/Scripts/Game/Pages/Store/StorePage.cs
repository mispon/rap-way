using Core;
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
        [SerializeField] private StoreItemsController swagStoreItemsController;
        [SerializeField] private StoreItemsController workStoreItemsController;
        
        [Header("Эффект открытия новой шмотки")]
        [SerializeField] private NewItemEffect newGoodEffect;
        
        [Header("Данные")]
        [SerializeField] private GoodsData data;

        protected override void BeforePageOpen()
        {
            //AppodealManager.Instance.ShowBanner();
            workStoreItemsController.Initialize(data.WorkTools, newGoodEffect);
            swagStoreItemsController.Initialize(data.Swag, newGoodEffect);
        }

        protected override void AfterPageClose()
        {
            workStoreItemsController.Dispose();
            swagStoreItemsController.Dispose();
            //AppodealManager.Instance.HideBanner();
        }
    }
}