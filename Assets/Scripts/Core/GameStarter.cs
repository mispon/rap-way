using Core.Interfaces;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Управляет упорядоченной инициализацией объектов сцены
    /// </summary>
    public class GameStarter : MonoBehaviour
    {
        [Header("Порядок инициализации")]
        [SerializeField] private MonoBehaviour[] starters;
        
        private void Start()
        {
            foreach (var starter in starters)
                (starter as IStarter)?.OnStart();
        }
    }
}