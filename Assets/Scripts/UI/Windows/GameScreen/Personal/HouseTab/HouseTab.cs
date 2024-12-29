using System;
using System.Linq;
using Game.Player.Character;
using Game.Player.Inventory.Desc;
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

        [Space] [Header("House")]
        [SerializeField] private HouseSettings[] settingsByLevel;
        [SerializeField] private GameObject houseGroup;
        [SerializeField] private Image      houseImage;

        [Space] [Header("Rooms")]
        [SerializeField] private GameObject[] houseRooms;

        public override void Open()
        {
            var house = PlayerAPI.Data.Inventory
                .Where(e => e.Type == InventoryType.House)
                .Select(e => e.Value<ValuesItem>())
                .OrderByDescending(e => e.Level)
                .FirstOrDefault();

            var houseExists = house.Level > 0;
            SetActiveGroup(houseExists);

            if (houseExists)
            {
                var settings = settingsByLevel[house.Level - 1];
                houseImage.sprite = settings.Image;

                for (var i = 0; i < settings.Rooms.Length; i++)
                {
                    var roomAvailability = settings.Rooms[i];
                    houseRooms[i].SetActive(roomAvailability);
                }
            }

            Character.Instance.Hide();
            base.Open();
        }

        private void SetActiveGroup(bool houseExists)
        {
            noHouseLabel.SetActive(!houseExists);
            houseGroup.SetActive(houseExists);
        }
    }
}