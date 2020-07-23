using Data;
using Game.Pages.Track;

namespace Game.Pages.Feat
{
    /// <summary>
    /// Страница настройки фита
    /// </summary>
    public class FeatSettingsPage : TrackSettingsPage
    {
        private RapperInfo _rapper;
        
        /// <summary>
        /// Открывает страницу настроек
        /// </summary>
        public void Show(RapperInfo rapper)
        {
            _rapper = rapper;
            Open();
        }

        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            base.BeforePageOpen();
            _track.Feat = _rapper;
        }

        protected override void AfterPageClose()
        {
            base.AfterPageClose();
            _rapper = null;
        }

        #endregion
    }
}