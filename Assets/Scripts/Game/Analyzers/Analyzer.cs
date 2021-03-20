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
        protected const float ONE_PERCENT = 0.01f;
        protected const float TEN_PERCENTS = 0.1f;

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
        /// Возвращает отношение прослушиваний к общему числу фанатов
        /// </summary>
        protected float GetListenRatio(int listensAmount)
        {
            return 1f * listensAmount / GetFans();
        }

        /// <summary>
        /// Возвращает количество фанатов
        /// </summary>
        protected int GetFans()
        {
            return Mathf.Max(PlayerManager.Data.Fans, settings.BaseFans);
        }

        /// <summary>
        /// Рандомизирует значения доходов
        /// </summary>
        protected (int fans, int money) CalculateIncomes(float quality, int activitiesCount, float activityCost)
        {
            // Фанаты - 10% от активности (просмотры, прослушиваня и т.д.)
            float fansRaw = Math.Min(activitiesCount * TEN_PERCENTS, settings.FansSignificantValue);
            int fans = Convert.ToInt32(fansRaw * quality);

            // Доход - количество прослушиваний * стоимость одного прослушивания
            int money = Convert.ToInt32(activitiesCount * activityCost);

            fans = Mathf.Min(fans, settings.MaxFansIncome);
            int fansRandomizer = Convert.ToInt32(fans * TEN_PERCENTS);

            money = Mathf.Min(money, settings.MaxMoneyIncome);
            int moneyRandomizer = Convert.ToInt32(money * TEN_PERCENTS);

            return (
                Random.Range(fans - fansRandomizer, fans + fansRandomizer),
                Random.Range(money - moneyRandomizer, money + moneyRandomizer)
            );
        }
    }
}