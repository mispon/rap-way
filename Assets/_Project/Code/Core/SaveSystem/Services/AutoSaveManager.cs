using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace RapWay.Core.SaveSystem.Services
{
    public class AutoSaveManager : MonoBehaviour
    {
        [SerializeField] private float autoSaveIntervalSeconds = 60f;
        
        private SaveLoadService _saveService;
        private bool _isActive = true;

        [Inject]
        public void Construct(SaveLoadService saveService)
        {
            _saveService = saveService;
        }

        private void Start()
        {
            // run endless autosave loop
            RunAutoSaveLoop().Forget();
        }

        private async UniTaskVoid RunAutoSaveLoop()
        {
            while (_isActive)
            {
                await UniTask.Delay((int)(autoSaveIntervalSeconds * 1000));
                
                if (!_isActive) break;

                await _saveService.SaveGameAsync();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            
            Debug.Log("[AutoSave] app paused - saving...");
            _saveService.SaveGameAsync().Forget();
        }

        private void OnApplicationQuit()
        {
            Debug.Log("[AutoSave] app quitting - saving...");
            _saveService.SaveGameAsync().Forget();
        }

        private void OnDestroy()
        {
            _isActive = false;
        }
    }
}