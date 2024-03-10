using System.Collections.Generic;
using System.Linq;
using Core.Localization;
using Enums;
using Extensions;
using Game.Player.Goods;
using Game.Player.State.Desc;
using Models.Production;
using ScriptableObjects;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Personal.PersonalTab
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
            PlayerData data = PlayerAPI.Data;

            SetupCharacter(data.Info.Gender);
            SetupGoods(data.Goods);
            SetupStats(data.Stats);
            SetupSkills(data.Skills);
            SetupBestTrack(data.History.TrackList);
            SetupLastActions(data.History.GetLastActions(3));
            
            base.Open();
            
            HintsManager.Instance.ShowHint("tutorial_personal_page", PersonalTabType.Personal);
        }

        private void SetupCharacter(Gender gender)
        {
            maleAvatar.SetActive(gender == Gender.Male);
            femaleAvatar.SetActive(gender == Gender.Female);
        }

        private void SetupGoods(List<Good> playerGoods) {
            Sprite GetGoodsSprite(GoodsType type)
            {
                var good = playerGoods
                    .OrderByDescending(e => e.Level)
                    .FirstOrDefault(e => e.Type == type);
                
                if (good == null)
                    return imageBank.Empty;

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

        private void SetupStats(PlayerStats playerStats)
        {
            vocobularyLevel.text = playerStats.Vocobulary.Value.ToString();
            bitmakingLevel.text = playerStats.Bitmaking.Value.ToString();
            flowLevel.text = playerStats.Flow.Value.ToString();
            charismaLevel.text = playerStats.Charisma.Value.ToString();
            managementLevel.text = playerStats.Management.Value.ToString();
            marketingLevel.text = playerStats.Marketing.Value.ToString();
        }

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

        private void SetupBestTrack(List<TrackInfo> trackList)
        {
            var bestTrack = trackList.OrderByDescending(e => e.ListenAmount).FirstOrDefault();
            bestTrackName.text = bestTrack != null 
                ? bestTrack.Name 
                : LocalizationManager.Instance.Get("no_tracks_yet").ToUpper();
            listenAmount.text = bestTrack != null ? bestTrack.ListenAmount.GetDisplay() : "0";
        }

        private void SetupLastActions(List<ProductionBase> actions) {
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