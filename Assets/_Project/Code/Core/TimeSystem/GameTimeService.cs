using _Project.Data.Settings;
using VContainer.Unity;

namespace RapWay.Core.TimeSystem
{
    public class GameTimeService : ITickable
    {
        private int TotalDays { get; set; }
        private float TimeScale { get; set; } = 1f;
        private bool IsPaused => TimeScale <= 0f;
        
        private float _timer;
        private readonly TimeConfig _config;

        public GameTimeService(TimeConfig config)
        {
            _config = config;
            TotalDays = 1;
        }
        
        public void Tick()
        {
            if (IsPaused) return;

            _timer += UnityEngine.Time.deltaTime * TimeScale;

            if (_timer >= _config.RealSecondsPerDay)
            {
                _timer -= _config.RealSecondsPerDay;
                AdvanceDay();
            }
        }

        private void AdvanceDay()
        {
            TotalDays += 1;
        }
        
        public (int Year, int Month, int Day) GetReadableDate()
        {
            int year = (TotalDays / _config.DaysInYear) + 1;
            int month = ((TotalDays % _config.DaysInYear) / _config.DaysInMonth) + 1;
            int day = (TotalDays % _config.DaysInMonth);
            
            if (day == 0) 
                day = _config.DaysInMonth;
            
            return (year, month, day);
        }
        
        #region TimeSpeedControl

        public void Pause() => TimeScale = 0f;
        public void PlayNormal() => TimeScale = 1f;
        public void PlayFast() => TimeScale = 3f;

        #endregion

    }
}
