using Data;
using Game.Effects;
using Models.UI;
using UnityEngine;
using Utils.Extensions;

namespace Game.Pages.Store
{
    /// <summary>
    /// Страница магазина
    /// Инициализирует шмокти следующего уровня в двух ScrollView (шмотки для работы и понты)
    /// </summary>
    public class StorePage: Page
    {
        [Header("Контроллеры управления UI-элементов")]
        [SerializeField] private ScrollItemsController scrollItemsController;
        
        [Header("Эффект открытия новой шмотки")]
        [SerializeField] private NewItemEffect newGoodEffect;
        
        [Header("Данные")]
        [SerializeField] private GoodsData data;

        [Header("Настройки урпавления отображением элементов GameScreen")] 
        [SerializeField] private CanvasGroup gameScreenCanvasGroup;
        [SerializeField] private CanvasGroupSettings hideCanvasGroupSettings;
        [SerializeField] private CanvasGroupSettings showCanvasGroupSettings;

        protected override void BeforePageOpen()
        {
            scrollItemsController.Initialize(data.AllItems, newGoodEffect);
            gameScreenCanvasGroup.Set(hideCanvasGroupSettings);
        }

        protected override void AfterPageClose()
        {
            gameScreenCanvasGroup.Set(showCanvasGroupSettings);
            scrollItemsController.Dispose();
        }
    }
}