using System;
using System.Reflection;
using NUnit.Framework;

namespace Tests.EditorMode
{
    public class TestUtils
    {
        internal static Func<object[], object> CreateStaticMethod<T>(string methodName) where T : new()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            
            T obj = new T();
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, flags);

            Assert.NotNull(methodInfo);
            return args => methodInfo.Invoke(obj, args);
        }
        
        internal static Func<object[], object> CreateMethod<T>(string methodName) where T : new()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            
            T obj = new T();
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, flags);

            Assert.NotNull(methodInfo);
            return args => methodInfo.Invoke(obj, args);
        }
    }
}