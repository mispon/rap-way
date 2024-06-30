using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.PropertyAttributes;
using Enums;
using Game.Effects;
using Game.Player.Team.Desc;
using MessageBroker;
using MessageBroker.Messages.SocialNetworks;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game.Player.Team
{
    public class TeamManager : Singleton<TeamManager>
    {
        [Header("Data")] [ArrayElementTitle("Type")]
        public TeammateInfo[] teammateInfos;

        [Header("New teammate effect")] [SerializeField]
        private NewItemEffect newTeammateEffect;

        public void TryUnlockTeammates()
        {
            var lockedTeammates = GetTeammates(e => e.IsEmpty);
            if (lockedTeammates.Length == 0)
            {
                return;
            }

            var fans           = PlayerAPI.Data.Fans;
            var lockedTeammate = lockedTeammates.FirstOrDefault(e => GetInfo(e.Type).FansToUnlock <= fans);

            if (lockedTeammate != null)
            {
                UnlockTeammate(lockedTeammate);
            }
        }

        public Teammate[] GetTeammates(Func<Teammate, bool> predicate)
        {
            return PlayerAPI.Data.Team.TeammatesArray
                .Where(predicate)
                .ToArray();
        }

        public static bool IsAvailable(TeammateType type)
        {
            var teammate = PlayerAPI.Data.Team.TeammatesArray
                .First(e => e.Type == type);

            return teammate.IsEmpty == false && teammate.HasPayment;
        }

        public int GetSalary(Teammate teammate)
        {
            var info = GetInfo(teammate.Type);
            return info.Salary[teammate.Skill.Value - 1];
        }

        private void UnlockTeammate(Teammate teammate)
        {
            teammate.Skill.Value = 1;
            teammate.HasPayment  = true;

            var info = GetInfo(teammate.Type);

            void Notification()
            {
                SoundManager.Instance.PlaySound(UIActionType.Achieve);

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

            MsgBroker.Instance.Publish(new EmailMessage
            {
                Title      = "new_teammate_header",
                Content    = "new_teammate_header",
                Sprite     = info.Avatar,
                Sender     = "team.assistant@mail.com",
                mainAction = Notification
            });
        }

        private TeammateInfo GetInfo(TeammateType type)
        {
            return teammateInfos.First(e => e.Type == type);
        }
    }
}