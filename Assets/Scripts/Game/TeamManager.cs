using System;
using System.Linq;
using Core;
using Game.Notifications;
using Game.Pages.Team;
using Game.Pages.Training;
using Models.Player;
using UnityEngine;
using Utils;

namespace Game
{
    /// <summary>
    /// Логика взаимодействия с командой игрока
    /// </summary>
    public class TeamManager: Singleton<TeamManager>
    {
        [Header("Данные команды")] 
        [ArrayElementTitle("Type")] public TeammateInfo[] teammateInfos;

        [Header("Страницы команды")]
        [SerializeField] private TeammateUnlockPage unlockTeammatePage;

        [Header("Страница тренировок")]
        [SerializeField] private TrainingMainPage trainingPage;

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
            return info.Salary[teammate.Skill.Value - 1];
        }
        
        /// <summary>
        /// Проверяет возможность получения нового тиммейта
        /// </summary>
        private void CheckNewTeammate()
        {
            var lockedTeammates = GetTeammates(e => e.IsEmpty);
            if (lockedTeammates.Length == 0)
                return;

            var fans = PlayerManager.Data.Fans;
            var lockedTeammate = lockedTeammates
                .FirstOrDefault(tm => teammateInfos.First(tmi => tmi.Type == tm.Type).FansToUnlock <= fans);

            if (lockedTeammate != null)
                UnlockTeammate(lockedTeammate);
        }

        /// <summary>
        /// Разблокирует тиммейта
        /// </summary>
        private void UnlockTeammate(Teammate teammate)
        {
            teammate.Skill.Value = 1;
            teammate.HasPayment = true;

            void Notification() => unlockTeammatePage.Show(teammate);
            NotificationManager.Instance.AddNotification(Notification);
        }

        /// <summary>
        /// Открывает страницу выплаты зарплат
        /// </summary>
        private void OnSalary()
        {
            if (GetTeammates(e => !e.IsEmpty).Length == 0)
                return;
            
            const int teamTab = 3;
            NotificationManager.Instance.AddNotification(() => trainingPage.OpenPage(teamTab));
        }

        /// <summary>
        /// Возвращает тиммейтов подходящих под условию
        /// </summary>
        private static Teammate[] GetTeammates(Func<Teammate, bool> predicate)
        {
            return PlayerManager.Data.Team.TeammatesArray
                .Where(predicate)
                .ToArray();
        }
    }
}