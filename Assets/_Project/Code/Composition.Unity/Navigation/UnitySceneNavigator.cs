using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RapWay.Application.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RapWay.Composition.Unity.Navigation
{
    public sealed class UnitySceneNavigator : ISceneNavigator
    {
        public async ValueTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("A scene name is required.", nameof(sceneName));
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                throw new InvalidOperationException($"Scene '{sceneName}' could not be loaded.");
            }

            await operation.ToUniTask(cancellationToken: cancellationToken);
        }
    }
}
