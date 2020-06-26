using System.Linq;
using Core;
using Game.Pages;
using Game.Pages.Team;
using Models.Player;
using UnityEngine;
using Utils;

namespace Game
{
    public class TeamManager: Singleton<TeamManager>
    {

        [Header("Teammate external info")] 
        [SerializeField, ArrayElementTitle("type")] private TeammateInfo[] teammateInfos;

        [Header("Teammate event pages")] 
        [SerializeField] private TeammateUnlockPage unlockTeammatePage;
        [SerializeField] private Page salaryPage;


        private void Start()
        {
            TimeManager.Instance.onDayLeft += CheckNewTeammate;
            TimeManager.Instance.onMonthLeft += OnSalary;
        }

        private void CheckNewTeammate()
        {
            //Массив еще недоступных тиммеейтов
            var teammates = GameManager.Instance.PlayerData.Team.TeammatesArray.Where(tm => tm.Skill == 0).ToArray();
            if(teammates.Length == 0)
                return;

            var fans = GameManager.Instance.PlayerData.Data.Fans;
            //Первый попавшийся тиммейт, которого можно открыть
            var unlockTeammate =
                teammates.FirstOrDefault(tm => teammateInfos.First(tmi => tmi.type == tm.type).fansToUnlock <= fans);
            
            
           if(unlockTeammate != null)
               unlockTeammatePage.Show(unlockTeammate);
        }

        private void OnSalary()
        {
            //Массив доступных тиммейтов
            var teammates = GameManager.Instance.PlayerData.Team.TeammatesArray.Where(tm => tm.Skill != 0).ToArray();
            if(teammates.Length == 0)
                return;
            
            //todo: работаем с панелью отображения зарплат
        }
    }
}