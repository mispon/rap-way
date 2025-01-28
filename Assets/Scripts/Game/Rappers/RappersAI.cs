using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Analytics;
using Game.Rappers.AI;
using Game.Rappers.Desc;
using UnityEngine;
using Random = UnityEngine.Random;

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

            if (Random.Range(0, 2) == 0)
            {
                // 50% change some rapper do action
                return;
            }
            
            try
            {
                // one random rapper do action per day
                var randomIdx = Random.Range(0, rappers.Length);
                _rappersAI.DoAction(rappers[randomIdx], _settings, imagesBank, concertData);
                
            } catch (Exception e)
            {
                Debug.LogError(e);
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
    }
}