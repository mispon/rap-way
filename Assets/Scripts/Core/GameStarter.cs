using System.Collections;
using Core.Interfaces;
using Game;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Управляет упорядоченной инициализацией объектов сцены
    /// </summary>
    public class GameStarter : MonoBehaviour
    {
        [Header("Порядок инициализации")]
        [SerializeField] private GameObject[] starters;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance.IsReady);
            
            foreach (var starter in starters)
            {
                starter.GetComponent<IStarter>().OnStart();
            }
        }
    }
}