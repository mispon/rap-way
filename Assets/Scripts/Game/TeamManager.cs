using System;
using System.Linq;
using Core;
using Game.Pages.Team;
using Models.Player;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Логика взаимодействия с командой игрока
    /// </summary>
    public class TeamManager: MonoBehaviour
    {
        [Header("Данные команды")] 
        [ArrayElementTitle("Type")] public TeammateInfo[] teammateInfos;

        [Header("Страницы команды")] 
        [SerializeField] private TeammateUnlockPage unlockTeammatePage;
        [SerializeField] private TeammateSalaryPage salaryPage;

        private void Start()
        {
            TimeManager.Instance.onDayLeft += CheckNewTeammate;
            TimeManager.Instance.onMonthLeft += OnSalary;
        }

        /// <summary>
        /// Возвращает зарплату тиммейта
        /// </summary>
        public int GetSalary(Teammate teammate)
        {
            var info = teammateInfos.First(tmi => tmi.Type == teammate.Type);
            return info.Salary[teammate.Skill - 1];
        }
        
        /// <summary>
        /// Проверяет возможность получения нового тиммейта
        /// </summary>
        private void CheckNewTeammate()
        {
            var lockedTeammates = GetTeammates(e => e.IsEmpty);
            if (lockedTeammates.Length == 0)
                return;

            var fans = PlayerManager.PlayerData.Data.Fans;
            var lockedTeammate = lockedTeammates
                .FirstOrDefault(tm => teammateInfos.First(tmi => tmi.Type == tm.Type).FansToUnlock <= fans);
            
           if (lockedTeammate != null)
               unlockTeammatePage.Show(lockedTeammate);
        }

        /// <summary>
        /// Открывает страницу выплаты зарплат
        /// </summary>
        private void OnSalary()
        {
            var teammates = GetTeammates(e => !e.IsEmpty);
            if (teammates.Length > 0)
                salaryPage.Show(teammates);
        }

        /// <summary>
        /// Возвращает тиммейтов подходящих под условию
        /// </summary>
        private static Teammate[] GetTeammates(Func<Teammate, bool> predicate)
        {
            return PlayerManager.PlayerData.Team.TeammatesArray
                .Where(predicate)
                .ToArray();
        }
    }
}