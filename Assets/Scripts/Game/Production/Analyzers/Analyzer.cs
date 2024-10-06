using System;
using UnityEngine;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Production.Analyzers
{
    public class Analyzer
    {
        protected static int CalcNewFansCount(int fans, float quality, int minIncome, int maxIncome)
        {
            var fansGrowVector = new[]
            {
                (1000, 0.3f),
                (5000, 0.25f),
                (10_000, 0.2f),
                (50_000, 0.15f),
                (100_000, 0.1f),
                (250_000, 0.08f),
                (500_000, 0.06f),
                (750_000, 0.04f),
                (1_000_000, 0.03f),
                (1_500_000, 0.025f),
                (2_500_000, 0.020f),
                (5_000_000, 0.015f),
                (7_500_000, 0.013f),
                (10_000_000, 0.01f),
                (20_000_000, 0.008f),
                (30_000_000, 0.007f),
                (50_000_000, 0.006f),
                (100_000_000, 0.004f),
                (250_000_000, 0.002f)
            };

            const float minPercent = 0.002f;
            var         percent    = minPercent;

            foreach (var (fansCount, percentValue) in fansGrowVector)
            {
                if (fans <= fansCount)
                {
                    percent = percentValue;
                    break;
                }
            }

            var factor  = percent * Mathf.Max(1.0f, 0.7f + quality);
            var newFans = Convert.ToInt32(fans * factor);
            newFans = Mathf.Clamp(newFans, minIncome, maxIncome);

            return AddFuzzing(newFans);
        }

        protected static int CalcMoneyIncome(int streams, float cost, int minIncome, int maxIncome)
        {
            var money = Convert.ToInt32(streams * cost);
            money = Mathf.Clamp(money, minIncome, maxIncome);

            return AddFuzzing(money);
        }

        protected static int AddFuzzing(int value)
        {
            const float tenPercents = 0.1f;

            var fuzz = Convert.ToInt32(value * tenPercents);
            if (value == int.MaxValue)
            {
                // to safe add back
                value -= fuzz;
            }

            return Random.Range(value - fuzz, value + fuzz);
        }

        protected static int GetFans(int creatorId, int baseFans)
        {
            return IsPlayerCreator(creatorId)
                ? Mathf.Max(PlayerAPI.Data.Fans, baseFans)
                : RappersAPI.Instance.Get(creatorId)?.Fans ?? 0;
        }

        protected static float GetHypeFactor(int creatorId)
        {
            return IsPlayerCreator(creatorId)
                ? Mathf.Max(0.1f, PlayerAPI.Data.Hype / 100f)
                : Random.Range(0.1f, 1.0f);
        }

        protected static bool IsPlayerCreator(int creatorId)
        {
            return creatorId == -1;
        }
    }
}