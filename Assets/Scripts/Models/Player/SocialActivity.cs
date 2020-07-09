using System;
using JetBrains.Annotations;

namespace Models.Player
{
    /// <summary>
    /// Класс контроля активации социального действия
    /// </summary>
    [Serializable]
    public class SocialActivity
    {
        /// <summary>
        /// Дней до активации социального действия
        /// </summary>
        private int _daysToActivate;
        /// <summary>
        /// При активации социального действия
        /// </summary>
        public Action ActivateAction;
        
        /// <summary>
        /// Активно ли социальное действие
        /// </summary>
        public bool IsActive => _daysToActivate <= 0;


        /// <summary>
        /// Установка дизактивации социального действия
        /// </summary>
        /// <param name="callback">Действие, выполняемое по истечению срока</param>
        public void SetDisable(int days, [CanBeNull] Action callback)
        {
            _daysToActivate = days;
            ActivateAction = callback;
        }

        public void OnDayLeft()
        {
            if (--_daysToActivate > 0)
                return;

            ActivateAction?.Invoke();
            ActivateAction = null;
        }
    }
}