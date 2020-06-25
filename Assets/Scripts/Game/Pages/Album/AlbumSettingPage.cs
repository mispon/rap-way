using Enums;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Pages.Album
{
    /// <summary>
    /// Страница настройки альбома
    /// </summary>
    public class AlbumSettingPage : Page
    {
        [Header("Контроллы")]
        [SerializeField] private InputField albumNameInput;
        [SerializeField] private Switcher themeSwitcher;
        [SerializeField] private Switcher styleSwitcher;
        [SerializeField] private Button startButton;

        [Header("Страница разработки")]
        [SerializeField] private AlbumWorkingPage workingPage;

        private AlbumInfo _album;
        
        private void Start()
        {
            albumNameInput.onValueChanged.AddListener(OnTrackNameInput);
            startButton.onClick.AddListener(CreateAlbum);
            
            themeSwitcher.InstantiateElements(PlayerManager.GetPlayersThemes());
            styleSwitcher.InstantiateElements(PlayerManager.GetPlayersStyles());
        }
        
        /// <summary>
        /// Обработчик ввода названия альбома 
        /// </summary>
        private void OnTrackNameInput(string value)
        {
            _album.Name = value;
        }
        
        /// <summary>
        /// Обработчик запуска работы над треком
        /// </summary>
        private void CreateAlbum()
        {
            var nickName = PlayerManager.PlayerData.Info.NickName;
            
            _album.Id = PlayerManager.GetNextProductionId<AlbumInfo>();
            _album.Theme = GetToneValue<Themes>(themeSwitcher);
            _album.Style = GetToneValue<Styles>(styleSwitcher);
            _album.Name = string.IsNullOrEmpty(_album.Name)
                ? $"{nickName} - Track {_album.Id}"
                : $"{nickName} - {_album.Name}";
            
            workingPage.CreateAlbum(_album);
            Close();
        }
        
        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            _album = new AlbumInfo();
        }

        protected override void AfterPageClose()
        {
            _album = null;
            
            albumNameInput.SetTextWithoutNotify(string.Empty);
            themeSwitcher.ResetActive();
            styleSwitcher.ResetActive();
        }

        #endregion
    }
}