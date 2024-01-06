﻿using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Enums;
using Localization;
using Models.Info.Production;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Personal.PersonalTab
{
    public class PersonalTab : Tab
    {
        [Header("Персонажи")]
        [SerializeField] private GameObject maleAvatar; 
        [SerializeField] private GameObject femaleAvatar; 
        
        [Header("Имущество игрока")]
        [SerializeField] private Image microIcon;
        [SerializeField] private Image acousticIcon;
        [SerializeField] private Image mixerIcon;
        [SerializeField] private Image soundCardIcon;
        [SerializeField] private Image carIcon;
        [SerializeField] private Image chainIcon;
        [SerializeField] private Image swatchesIcon;
        [SerializeField] private Image grillzIcon;

        [Header("Навыки персонажа")]
        [SerializeField] private Text vocobularyLevel;
        [SerializeField] private Text bitmakingLevel;
        [SerializeField] private Text flowLevel;
        [SerializeField] private Text charismaLevel;
        [SerializeField] private Text managementLevel;
        [SerializeField] private Text marketingLevel;

        [Header("Умения персонажа")]
        [SerializeField] private Image[] skillsIcons;
        [SerializeField] private GameObject noSkills;
        
        [Header("Лучший трек")]
        [SerializeField] private Text bestTrackName;
        [SerializeField] private Text listenAmount;

        [Header("Последние действия")]
        [SerializeField] private Text[] lastActions;

        [Header("Данные")]
        [SerializeField] private GoodsData goods;
        [SerializeField] private ImagesBank imageBank;
        
        public override void Open()
        {
            PlayerData data = PlayerManager.Data;

            SetupCharacter(data.Info.Gender);
            SetupGoods(data.Goods);
            SetupStats(data.Stats);
            SetupSkills(data.Skills);
            SetupBestTrack(data.History.TrackList);
            SetupLastActions(data.History.GetLastActions(3));
            
            base.Open();
            
            TutorialManager.Instance.ShowTutorial("tutorial_personal_page");
        }
        
        /// <summary>
        /// Устанавливает аватар 
        /// </summary>
        private void SetupCharacter(Gender gender)
        {
            maleAvatar.SetActive(gender == Gender.Male);
            femaleAvatar.SetActive(gender == Gender.Female);
        }

        /// <summary>
        /// Устанавливает имущество
        /// </summary>
        private void SetupGoods(List<Good> playerGoods) {
            Sprite GetGoodsSprite(GoodsType type)
            {
                var good = playerGoods.FirstOrDefault(e => e.Type == type);
                if (good == null)
                    return null;

                return goods.Items[type]
                    .First(e => e.Level == good.Level)
                    .PersonalPageImage;
            }

            microIcon.sprite = GetGoodsSprite(GoodsType.Micro);
            soundCardIcon.sprite = GetGoodsSprite(GoodsType.AudioCard);
            mixerIcon.sprite = GetGoodsSprite(GoodsType.FxMixer);
            acousticIcon.sprite = GetGoodsSprite(GoodsType.Acoustic);
            carIcon.sprite = GetGoodsSprite(GoodsType.Car);
            swatchesIcon.sprite = GetGoodsSprite(GoodsType.Swatches);
            chainIcon.sprite = GetGoodsSprite(GoodsType.Chain);
            grillzIcon.sprite = GetGoodsSprite(GoodsType.Grillz);
        }
        
        /// <summary>
        /// Устанавливает навыки персонажа
        /// </summary>
        private void SetupStats(PlayerStats playerStats)
        {
            vocobularyLevel.text = playerStats.Vocobulary.Value.ToString();
            bitmakingLevel.text = playerStats.Bitmaking.Value.ToString();
            flowLevel.text = playerStats.Flow.Value.ToString();
            charismaLevel.text = playerStats.Charisma.Value.ToString();
            managementLevel.text = playerStats.Management.Value.ToString();
            marketingLevel.text = playerStats.Marketing.Value.ToString();
        }

        /// <summary>
        /// Устанавливает умения персонажа
        /// </summary>
        private void SetupSkills(List<Skills> playerSkills)
        {
            noSkills.SetActive(playerSkills.Count == 0);
            
            for (int i = 0; i < skillsIcons.Length; i++)
            {
                Image icon = skillsIcons[i];
                icon.gameObject.SetActive(i < playerSkills.Count);
                
                if (icon.gameObject.activeSelf)
                    skillsIcons[i].sprite = imageBank.Skills[(int) playerSkills[i]];    
            }
        }

        /// <summary>
        /// Устанавливает самый успешный трек
        /// </summary>
        private void SetupBestTrack(List<TrackInfo> trackList)
        {
            var bestTrack = trackList.OrderByDescending(e => e.ListenAmount).FirstOrDefault();
            bestTrackName.text = bestTrack != null 
                ? bestTrack.Name 
                : LocalizationManager.Instance.Get("no_tracks_yet").ToUpper();
            listenAmount.text = bestTrack != null ? bestTrack.ListenAmount.GetDisplay() : "0";
        }

        /// <summary>
        /// Выводит последние действия
        /// </summary>
        private void SetupLastActions(List<Production> actions) {
            for (var i = 0; i < lastActions.Length; i++) {
                GameObject item = lastActions[i].transform.parent.gameObject;
                item.SetActive(i < actions.Count);
                if (item.activeSelf)
                {
                    lastActions[i].text = actions[i].GetLog();
                }
            }
        }
    }
}