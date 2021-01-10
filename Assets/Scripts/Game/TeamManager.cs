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
using EventType = Core.EventType;

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
        [SerializeField] private NewItemEffect newTeammateEffect;

        private void Start()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.onMonthLeft += OnMonthLeft;
        }

        /// <summary>
        /// Проверяет доступность тиммейта
        /// </summary>
        public static bool IsAvailable(TeammateType type)
        {
            var teammate = PlayerManager.Data.Team.TeammatesArray
                .First(e => e.Type == type);
            
            return teammate.IsEmpty == false && teammate.HasPayment;
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
        private void OnDayLeft()
        {
            CheckTeammatesUnlock();
            DecreaseManagerCooldown();
        }
        
        /// <summary>
        /// Открывает страницу выплаты зарплат
        /// </summary>
        private void OnMonthLeft()
        {
            var teammates = GetTeammates(e => !e.IsEmpty);
            if (teammates.Length == 0)
                return;

            foreach (var teammate in teammates)
                teammate.HasPayment = false;

            const int teamTab = 3;
            NotificationManager.Instance.AddClickNotification(() => trainingPage.OpenPage(teamTab));
            EventManager.RaiseEvent(EventType.UncleSamsParty);
        }

        /// <summary>
        /// Проверяет возможность разблокировки новых тиммейтов
        /// </summary>
        private void CheckTeammatesUnlock()
        {
            var lockedTeammates = GetTeammates(e => e.IsEmpty);
            if (lockedTeammates.Length == 0)
                return;

            int fans = PlayerManager.Data.Fans;
            var lockedTeammate = lockedTeammates.FirstOrDefault(e => GetInfo(e.Type).FansToUnlock <= fans);

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
                SoundManager.Instance.PlayAchieve();
                
                var info = GetInfo(teammate.Type);
                newTeammateEffect.Show(info.Avatar, () => unlockTeammatePage.Show(teammate, info.Avatar));
            }
            
            NotificationManager.Instance.AddClickNotification(Notification);
        }

        /// <summary>
        /// Сокращает кулдаун менеджера
        /// </summary>
        private static void DecreaseManagerCooldown()
        {
            var manager = PlayerManager.Data.Team.Manager;
            if (manager.Cooldown > 0)
                manager.Cooldown -= 1;
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

        private void OnDestroy()
        {
            //ToDo: (Андрей). Не уверен, что это необходимо.
            // 1. Нет никаких гарантий, что TeamManager.OnDestroy() вызовется раньше, чем TimeManamger.OnDestroy().
            //    В противном случае вылетит NullReferenceException
            // 2. TeamManager не существует нигде отдельно от TimeManager и наоброт.
            //     Следовательно, разрушение любого из них сопровождается разрушением другого => вызов событий TimeManager'a не должен возникать
            // 3. Если по-прежднему хочется отписываться, то лучше это делать OnDisable, так как тут гарантировано, что TimeManager.Instance все еще существует.
        
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.onMonthLeft -= OnMonthLeft;
        }
    }
}