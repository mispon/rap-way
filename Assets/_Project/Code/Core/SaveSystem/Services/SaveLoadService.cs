using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using RapWay.Core.SaveSystem.Data;
using RapWay.Domain.Interfaces;
using UnityEngine;

namespace RapWay.Core.SaveSystem.Services
{
    public class SaveLoadService
    {
        private const string SAVE_FILE_NAME = "savegame.json";
        
        private readonly IStorageService _storage;
        private readonly List<ISaveable> _saveables = new();
        
        private GameSaveProfile _currentProfile;

        public SaveLoadService(IStorageService storage)
        {
            _storage = storage;
            _currentProfile = new GameSaveProfile();
        }
        
        public void Register(ISaveable saveable)
        {
            if (!_saveables.Contains(saveable))
            {
                _saveables.Add(saveable);
            }
        }

        public void Unregister(ISaveable saveable)
        {
            _saveables.Remove(saveable);
        }
        
        public async UniTask SaveGameAsync()
        {
            foreach (var saveable in _saveables)
            {
                _currentProfile.State[saveable.SaveId] = saveable.CaptureState();
            }

            _currentProfile.LastSaveTime = System.DateTime.Now.ToString(CultureInfo.InvariantCulture);
            
            await _storage.SaveAsync(SAVE_FILE_NAME, _currentProfile);
            Debug.Log("[SaveSystem] game saved");
        }
        
        public async UniTask LoadGameAsync()
        {
            var loadedData = await _storage.LoadAsync<GameSaveProfile>(SAVE_FILE_NAME);

            if (loadedData == null)
            {
                Debug.Log("[SaveSystem] save file not found");
                _currentProfile = new GameSaveProfile();
                return;
            }

            _currentProfile = loadedData;
            
            foreach (var saveable in _saveables)
            {
                if (_currentProfile.State.TryGetValue(saveable.SaveId, out object rawState))
                {
                    saveable.RestoreState(rawState);
                }
            }
            Debug.Log("[SaveSystem] game loaded");
        }
    }
}