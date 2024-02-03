using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Core
{
    public class UnityServicesInitializer : MonoBehaviour
    {
        [SerializeField] private string environment = "production";

        public async Task Initialize()
        {
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(environment);
                await UnityServices.InitializeAsync(options);
                Debug.Log("UnityServicesInitializer: OK");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}