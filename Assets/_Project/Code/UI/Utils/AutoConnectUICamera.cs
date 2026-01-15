using UnityEngine;

namespace RapWay.UI.Utils
{
    [RequireComponent(typeof(Canvas))]
    public class AutoConnectUICamera : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            
            if (canvas.worldCamera != null) 
                return;

            var uiCamObj = GameObject.FindGameObjectWithTag("UICamera");
            if (uiCamObj != null)
            {
                canvas.worldCamera = uiCamObj.GetComponent<Camera>();
            }
            else
            {
                Debug.LogError($"[AutoConnectUICamera] unable to find camera with tag UICamera for {gameObject.name}");
            }
        }
    }
}