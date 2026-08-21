using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization
{
    static class AsyncOperationUtility
    {
        public static T SynchronousLoad<T>(AsyncOperationHandle<T> op)
        {
            return op.IsDone ? op.Result : op.WaitForCompletion();
        }
    }
}
