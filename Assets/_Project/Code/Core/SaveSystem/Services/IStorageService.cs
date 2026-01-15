using Cysharp.Threading.Tasks;

namespace RapWay.Core.SaveSystem.Services
{
    public interface IStorageService
    {
        UniTask SaveAsync(string fileName, object data);
        UniTask<T> LoadAsync<T>(string fileName);
    }
}