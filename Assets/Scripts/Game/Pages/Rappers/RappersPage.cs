using UnityEngine;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Страница реальных реперов
    /// </summary>
    public class RappersPage : Page
    {
        [SerializeField] private RappersGrid grid;
        [SerializeField] private RapperCard card;
        
        protected override void BeforePageOpen()
        {
            grid.Init();
            card.gameObject.SetActive(false);
        }
    }
}