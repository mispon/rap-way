using System;
using _Project.Data.Settings;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RapWay.Core.SaveSystem.Services;
using RapWay.Domain.Interfaces;
using VContainer.Unity;

namespace RapWay.Core.TimeSystem
{
    [Serializable]
    public class TimeSaveData
    {
        public int TotalDays;
    }
    
    public class GameTimeService : ITickable, ISaveable
    {
        public string SaveId => "World_Time";

        private int TotalDays { get; set; }
        private float TimeScale { get; set; } = 1f;
        private bool IsPaused => TimeScale <= 0f;
        
        private float _timer;
        private readonly TimeConfig _config;
        private readonly SaveLoadService _saveSystem;
        
        public GameTimeService(TimeConfig config, SaveLoadService saveSystem)
        {
            _config = config;
            
            _saveSystem = saveSystem;
            _saveSystem.Register(this);
            
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

            // on month passed
            if (TotalDays % 30 == 0)
            {
                _saveSystem.SaveGameAsync().Forget(); 
            }
            
            // on year passed
            if (TotalDays % 365 == 0)
            {
                _saveSystem.SaveGameAsync().Forget(); 
            }
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

        #region SaveSystem

        public object CaptureState()
        {
            return new TimeSaveData
            {
                TotalDays = TotalDays
            };
        }

        public void RestoreState(object state)
        {
            var data = ((JObject)state).ToObject<TimeSaveData>();
            TotalDays =  data.TotalDays;
            _timer = 0;
        }

        #endregion
    }
}
