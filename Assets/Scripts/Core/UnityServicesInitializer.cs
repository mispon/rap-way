using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Core
{
    public class UnityServicesInitializer : MonoBehaviour
    {
        [SerializeField] private string environment = "production";

        private async void Start()
        {
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(environment);
                await UnityServices.InitializeAsync(options);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}