using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.PropertyAttributes;
using Enums;
using Game.Effects;
using Game.Notifications;
using Game.Player.Team.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Player.Team
{
    /// <summary>
    /// Логика взаимодействия с командой игрока
    /// </summary>
    public class TeamManager: Singleton<TeamManager>
    {
        [Header("Данные команды")] 
        [ArrayElementTitle("Type")] public TeammateInfo[] teammateInfos;

        [Header("Эффект открытия нового тиммейта")]
        [SerializeField] private NewItemEffect newTeammateEffect;

        /// <summary>
        /// Checks and unlock available teammates
        /// </summary>
        public void TryUnlockTeammates()
        {
            var lockedTeammates = GetTeammates(e => e.IsEmpty);
            if (lockedTeammates.Length == 0)
                return;

            int fans = PlayerPackage.Data.Fans;
            var lockedTeammate = lockedTeammates.FirstOrDefault(e => GetInfo(e.Type).FansToUnlock <= fans);

            if (lockedTeammate != null)
                UnlockTeammate(lockedTeammate);
        }
        
        /// <summary>
        /// Returns teammate by predicate
        /// </summary>
        public Teammate[] GetTeammates(Func<Teammate, bool> predicate)
        {
            return PlayerAPI.Data.Team.TeammatesArray
                .Where(predicate)
                .ToArray();
        }
        
        /// <summary>
        /// Check teammate availability
        /// </summary>
        public static bool IsAvailable(TeammateType type)
        {
            var teammate = PlayerAPI.Data.Team.TeammatesArray
                .First(e => e.Type == type);
            
            return teammate.IsEmpty == false && teammate.HasPayment;
        }

        /// <summary>
        /// Returns teammate salary
        /// </summary>
        public int GetSalary(Teammate teammate)
        {
            var info = GetInfo(teammate.Type);
            return info.Salary[teammate.Skill.Value - 1];
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
                SoundManager.Instance.PlaySound(UIActionType.Achieve);
                
                var info = GetInfo(teammate.Type);
                newTeammateEffect.Show(info.Avatar, () =>
                {
                    MsgBroker.Instance.Publish(new WindowControlMessage
                    {
                        Context = new Dictionary<string, object>
                        {
                            ["teammate"] = teammate,
                            ["sprite"]   = info.Avatar 
                        }
                    });
                });
            }
            
            NotificationManager.Instance.AddClickNotification(Notification);
        }
        
        /// <summary>
        /// Возвращает информацию о тиммейте 
        /// </summary>
        private TeammateInfo GetInfo(TeammateType type)
        {
            return teammateInfos.First(e => e.Type == type);
        }
    }
}