using System;
using Core.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Analyzers
{
    /// <summary>
    /// Базовый анализатор
    /// </summary>
    public abstract class Analyzer<T> : MonoBehaviour
    {
        protected GameSettings settings;

        private void Start()
        {
            settings = GameManager.Instance.Settings;
        }

        /// <summary>
        /// Анализирует успешность деятельности игрока 
        /// </summary>
        public abstract void Analyze(T social);

        /// <summary>
        /// Рассчитывает прирост фанатов
        /// </summary>
        protected int CalcNewFansCount(int fans, float quality)
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
                (250_000_000, 0.002f),
            };

            const float minPercent = 0.002f;
            var percent = minPercent;

            foreach (var (fansCount, percentValue) in fansGrowVector)
            {
                if (fans <= fansCount)
                {
                    percent = percentValue;
                    break;
                }
            }

            float factor = percent * Mathf.Max(1.0f, 0.7f + quality);
            int newFans = Convert.ToInt32(fans * factor);

            newFans = Math.Max(settings.MinFansIncome, newFans);
            newFans = Math.Min(settings.MaxFansIncome, newFans);
            
            return AddFuzzing(newFans);
        }

        /// <summary>
        /// Рассчитывает прирос денег
        /// </summary>
        protected int CalcMoneyIncome(int streams, float cost)
        {
            int money = Convert.ToInt32(streams * cost);
            
            money = Math.Max(10, money);
            money = Math.Min(settings.MaxMoneyIncome, money);
            
            return AddFuzzing(money);
        }
        
        /// <summary>
        /// Добавляет рандомное отклонение на N%
        /// </summary>
        protected static int AddFuzzing(int value)
        {
            const float tenPercents = 0.1f;
            int fuzz = Convert.ToInt32(value * tenPercents);
            return Random.Range(value - fuzz, value + fuzz);
        }

        /// <summary>
        /// Возвращает количество фанатов
        /// </summary>
        protected int GetFans()
        {
            return Mathf.Max(PlayerManager.Data.Fans, settings.BaseFans);
        }
        
        /// <summary>
        /// Возвращает коэффициент хайпа
        /// </summary>
        protected float GetHypeFactor()
        {
            return Mathf.Max(0.1f, PlayerManager.Data.Hype / 100f);
        }
    }
}