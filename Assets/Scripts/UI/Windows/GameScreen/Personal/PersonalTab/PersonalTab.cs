using System.Collections.Generic;
using System.Linq;
using Core.Localization;
using Enums;
using Extensions;
using Game.Player.Character;
using Game.Player.Inventory.Desc;
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
        [Header("Player Goods")]
        [SerializeField] private Image microIcon;
        [SerializeField] private Image acousticIcon;
        [SerializeField] private Image mixerIcon;
        [SerializeField] private Image soundCardIcon;
        [SerializeField] private Image carIcon;
        [SerializeField] private Image chainIcon;
        [SerializeField] private Image swatchesIcon;
        [SerializeField] private Image grillzIcon;

        [Header("Player Abilities")]
        [SerializeField] private Text vocobularyLevel;
        [SerializeField] private Text bitmakingLevel;
        [SerializeField] private Text flowLevel;
        [SerializeField] private Text charismaLevel;
        [SerializeField] private Text managementLevel;
        [SerializeField] private Text marketingLevel;

        [Header("Player Skills")]
        [SerializeField] private Image[] skillsIcons;
        [SerializeField] private GameObject noSkills;

        [Header("Best Track")]
        [SerializeField] private Text bestTrackName;
        [SerializeField] private Text listenAmount;

        [Header("Last Actions")]
        [SerializeField] private Text[] lastActions;

        [Header("Data")]
        [SerializeField] private StoreData storeBank;
        [SerializeField] private ImagesBank imageBank;

        public override void Open()
        {
            HintsManager.Instance.ShowHint("tutorial_personal_page", PersonalTabType.Personal);

            SetupGoods(PlayerAPI.Data.Inventory);
            SetupStats(PlayerAPI.Data.Stats);
            SetupSkills(PlayerAPI.Data.Skills);
            SetupBestTrack(PlayerAPI.Data.History.TrackList);
            SetupLastActions(PlayerAPI.Data.History.GetLastActions(3));

            Character.Instance.Show(0.8f);
            base.Open();
        }

        private void SetupGoods(List<InventoryItem> inventory)
        {
            Sprite GetGoodsSprite(InventoryType type)
            {
                var item = inventory
                    .Where(e => e.Type == type)
                    .OrderByDescending(e => e.Value<ValuesItem>().Quality)
                    .ThenBy(e => e.Value<ValuesItem>().Hype)
                    .FirstOrDefault();

                if (item == null)
                {
                    return imageBank.Empty;
                }

                var itemsGroup = storeBank.Groups.First(e => e.Type == item.Type);

                return itemsGroup.Items
                    .First(e => e.Name == item.Name)
                    .PersonalPageImage;
            }

            microIcon.sprite     = GetGoodsSprite(InventoryType.Micro);
            soundCardIcon.sprite = GetGoodsSprite(InventoryType.AudioCard);
            mixerIcon.sprite     = GetGoodsSprite(InventoryType.FxMixer);
            acousticIcon.sprite  = GetGoodsSprite(InventoryType.Acoustic);
            carIcon.sprite       = GetGoodsSprite(InventoryType.Car);
            swatchesIcon.sprite  = GetGoodsSprite(InventoryType.Swatches);
            chainIcon.sprite     = GetGoodsSprite(InventoryType.Chain);
            grillzIcon.sprite    = GetGoodsSprite(InventoryType.Grillz);
        }

        private void SetupStats(PlayerStats playerStats)
        {
            vocobularyLevel.text = playerStats.Vocobulary.Value.ToString();
            bitmakingLevel.text  = playerStats.Bitmaking.Value.ToString();
            flowLevel.text       = playerStats.Flow.Value.ToString();
            charismaLevel.text   = playerStats.Charisma.Value.ToString();
            managementLevel.text = playerStats.Management.Value.ToString();
            marketingLevel.text  = playerStats.Marketing.Value.ToString();
        }

        private void SetupSkills(List<Skills> playerSkills)
        {
            noSkills.SetActive(playerSkills.Count == 0);

            for (var i = 0; i < skillsIcons.Length; i++)
            {
                var icon = skillsIcons[i];
                icon.gameObject.SetActive(i < playerSkills.Count);

                if (icon.gameObject.activeSelf)
                {
                    skillsIcons[i].sprite = imageBank.Skills[(int) playerSkills[i]];
                }
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

        private void SetupLastActions(List<ProductionBase> actions)
        {
            for (var i = 0; i < lastActions.Length; i++)
            {
                var item = lastActions[i].transform.parent.gameObject;
                item.SetActive(i < actions.Count);
                if (item.activeSelf)
                {
                    lastActions[i].text = actions[i].GetLog();
                }
            }
        }
    }
}