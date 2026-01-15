using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace RapWay.Core.SaveSystem.Services
{
    public class FileStorageService : IStorageService
    {
        private readonly string _basePath;

        public FileStorageService()
        {
            _basePath = Application.persistentDataPath;
        }

        public async UniTask SaveAsync(string fileName, object data)
        {
            var path = Path.Combine(_basePath, fileName);
            
            var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            await using var writer = new StreamWriter(path);
            await writer.WriteAsync(json);
        }

        public async UniTask<T> LoadAsync<T>(string fileName)
        {
            var path = Path.Combine(_basePath, fileName);
            if (!File.Exists(path)) return default;

            using var reader = new StreamReader(path);
            var json = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}