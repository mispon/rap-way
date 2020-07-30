using System;
using System.Linq;
using Core;
using Enums;
using Game.Effects;
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

        [Header("Страницы")]
        [SerializeField] private TeammateUnlockPage unlockTeammatePage;
        [SerializeField] private TrainingMainPage trainingPage;

        [Header("Эффект открытия нового тиммейта")]
        [SerializeField] private NewTeammateEffect newTeammateEffect;

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
            var info = GetInfo(teammate.Type);
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
                .FirstOrDefault(tm => GetInfo(tm.Type).FansToUnlock <= fans);

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

            void Notification()
            {
                var info = GetInfo(teammate.Type);
                newTeammateEffect.Show(
                    info.Avatar,
                    () => unlockTeammatePage.Show(teammate, info.Avatar)
                );
            }
            
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
        /// Возвращает информацию о тиммейте 
        /// </summary>
        private TeammateInfo GetInfo(TeammateType type)
        {
            return teammateInfos.First(e => e.Type == type);
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