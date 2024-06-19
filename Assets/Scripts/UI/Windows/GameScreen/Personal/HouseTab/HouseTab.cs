using System;
using System.Linq;
using Enums;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Personal.HouseTab
{
    [Serializable]
    public class HouseSettings
    {
        public Sprite Image;
        public bool[] Rooms;
    }
    
    public class HouseTab : Tab
    {
        [SerializeField] private GameObject noHouseLabel;
        [Space, Header("House")]
        [SerializeField] private HouseSettings[] settingsByLevel;
        [SerializeField] private GameObject houseGroup;
        [SerializeField] private Image houseImage;
        [Space, Header("Rooms")]
        [SerializeField] private GameObject[] houseRooms;
        
        public override void Open()
        {
            var house = PlayerAPI.Data.Goods
                .OrderByDescending(e => e.Level)
                .FirstOrDefault(e => e.Type == GoodsType.Apartments);
            
            SetState(isHouseExists: house != null);
            if (house != null) {
                var settings = settingsByLevel[house.Level-1];
                houseImage.sprite = settings.Image;

                for (var i = 0; i < settings.Rooms.Length; i++)
                {
                    var roomAvailability = settings.Rooms[i];
                    houseRooms[i].SetActive(roomAvailability);
                }
            }
            
            base.Open();
            
            // HintsManager.Instance.ShowHint("tutorial_house_page", PersonalTabType.House);
        }

        private void SetState(bool isHouseExists)
        {
            noHouseLabel.SetActive(!isHouseExists);
            houseGroup.SetActive(isHouseExists);
        }
    }
}