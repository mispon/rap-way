using System.Collections.Generic;
using System.Linq;
using Game.Rappers.AI;
using Game.Rappers.Desc;
using UnityEngine;

namespace Game.Rappers
{
    /// <summary>
    ///     Base AI controller
    /// </summary>
    public partial class RappersPackage
    {
        private readonly RappersAI _rappersAI = new();

        private void TriggerAIActions()
        {
            var rappers = GetAll().ToArray();
            DecrementCooldowns(rappers);

            // 3 random rappers do action per day
            var dices = ThrowDices(3, rappers.Length);
            foreach (var dice in dices)
            {
                var rapper = rappers[dice];
                _rappersAI.DoAction(rapper, _settings, concertData);
            }
        }

        private static void DecrementCooldowns(IEnumerable<RapperInfo> rappers)
        {
            foreach (var rapper in rappers)
            {
                if (rapper.Cooldown > 0)
                {
                    rapper.Cooldown--;
                }
            }
        }

        private static int[] ThrowDices(int n, int elemsTotal)
        {
            var set = new HashSet<int>();

            while (set.Count < n)
            {
                var dice = Random.Range(0, elemsTotal);
                set.Add(dice);
            }

            return set.ToArray();
        }
    }
}