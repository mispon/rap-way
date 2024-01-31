﻿using Core;
using Enums;
using Firebase.Analytics;
using Game;
using Game.Labels;
using Game.Player;
using Models.Player;
using Models.Production;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.Pages.Track
{
    /// <summary>
    /// Страница работы над треком
    /// </summary>
    public class TrackWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text bitPoints;
        [SerializeField] private Text textPoints;

        [Header("Команда игрока")]
        [SerializeField] private WorkPoints playerBitWorkPoints;
        [SerializeField] private WorkPoints playerTextWorkPoints;
        [SerializeField] private WorkPoints bitmakerWorkPoints;
        [SerializeField] private WorkPoints textwritterWorkPoints;
        [SerializeField] private WorkPoints labelBitWorkPoints;
        [SerializeField] private WorkPoints labelTextWorkPoints;
        [SerializeField] private Image bitmakerAvatar;
        [SerializeField] private Image textwritterAvatar;
        [SerializeField] private Image labelAvatar;
        [SerializeField] private GameObject labelFrozen;

        [Header("Данные")]
        [SerializeField] private ImagesBank imagesBank;

        [Header("Страница результата")]
        [SerializeField] private TrackResultPage trackResult;

        private TrackInfo _track;
        private bool _hasBitmaker;
        private bool _hasTextWriter;
        private LabelInfo _label;

        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.CreateTrackClick);
            
            _track = (TrackInfo) args[0];
            Open();
            RefreshWorkAnims();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        /// <summary>
        /// Обработчик перехода к странице результата
        /// </summary>
        private void ShowResultPage()
        {
            trackResult.Show(_track);
            Close();
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            GameEventsManager.Instance.CallEvent(GameEventType.Track, ShowResultPage);
        }

        /// <summary>
        /// Возвращает длительность действия
        /// </summary>
        protected override int GetDuration()
        {
            return settings.TrackWorkDuration;
        }

        /// <summary>
        /// Генерирует очки работы над треком
        /// </summary>
        private void GenerateWorkPoints()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
            
            _track.BitPoints +=  CreateBitPoints(PlayerManager.Data);
            _track.TextPoints += CreateTextPoints(PlayerManager.Data);
        }

        /// <summary>
        /// Создает очки работы по биту
        /// </summary>
        private int CreateBitPoints(PlayerData data)
        {
            var playersBitPoints = Random.Range(1, data.Stats.Bitmaking.Value + 2);
            playerBitWorkPoints.Show(playersBitPoints);

            var bitmakerPoints = 0;
            if (_hasBitmaker)
            {
                bitmakerPoints = Random.Range(1, data.Team.BitMaker.Skill.Value + 2);
                bitmakerWorkPoints.Show(bitmakerPoints);
            }
            
            var labelPoints = 0;
            if (_label is {IsFrozen: false})
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelBitWorkPoints.Show(labelPoints);
            }

            return playersBitPoints + bitmakerPoints + labelPoints;
        }
        
        /// <summary>
        /// Создает очки работы по тексту 
        /// </summary>
        private int CreateTextPoints(PlayerData data)
        {
            var playersTextPoints = Random.Range(1, data.Stats.Vocobulary.Value + 2);
            playerTextWorkPoints.Show(playersTextPoints);

            var textWriterPoints = 0;
            if (_hasTextWriter)
            {
                textWriterPoints = Random.Range(1, data.Team.TextWriter.Skill.Value + 2);
                textwritterWorkPoints.Show(textWriterPoints);
            }
            
            var labelPoints = 0;
            if (_label is {IsFrozen: false})
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelTextWorkPoints.Show(labelPoints);
            }

            return playersTextPoints + textWriterPoints + labelPoints;
        }

        /// <summary>
        /// Обновляет значение рабочих очков в интерфейсе
        /// </summary>
        private void DisplayWorkPoints()
        {
            bitPoints.text = _track.BitPoints.ToString();
            textPoints.text = _track.TextPoints.ToString();
        }

        protected override void BeforePageOpen()
        {
            base.BeforePageOpen();
            
            _hasBitmaker = TeamManager.IsAvailable(TeammateType.BitMaker);
            _hasTextWriter = TeamManager.IsAvailable(TeammateType.TextWriter);
            
            if (!string.IsNullOrEmpty(PlayerManager.Data.Label))
            {
                _label = LabelsManager.Instance.GetLabel(PlayerManager.Data.Label);
            }

            if (_label != null)
            {
                labelAvatar.gameObject.SetActive(true);
                labelAvatar.sprite = _label.Logo;
                labelFrozen.SetActive(_label.IsFrozen);
            } else
            {
                labelAvatar.gameObject.SetActive(false);
            }
            
            bitmakerAvatar.sprite = _hasBitmaker ? imagesBank.BitmakerActive : imagesBank.BitmakerInactive;
            textwritterAvatar.sprite = _hasTextWriter ? imagesBank.TextwritterActive : imagesBank.TextwritterInactive;
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();
            
            bitPoints.text = textPoints.text = "0";
            _track = null;
        }
    }
}