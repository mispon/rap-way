using System.Collections;
using Game;
using UnityEngine;

namespace Core.OrderedStarter
{
    /// <summary>
    /// Управляет упорядоченной инициализацией объектов сцены
    /// </summary>
    public class OrderedStarter : MonoBehaviour
    {
        [Header("Порядок инициализации")]
        [SerializeField] private MonoBehaviour[] starters;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameManager.Instance.IsReady);

            foreach (var starter in starters)
                (starter as IStarter)?.OnStart();
        }
    }
}