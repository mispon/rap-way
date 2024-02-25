using System.Collections.Generic;

namespace Core.Context
{
    public static class Context
    {
        public static T Value<T>(this object ctx)
        {
            if (ctx is T val)
            {
                return val;
            }
            
            return default;
        }

        public static T ValueByKey<T>(this object ctx, string key)
        {
            if (ctx is Dictionary<string, object> ctxMap)
            {
                if (ctxMap.TryGetValue(key, out var obj) && obj is T val)
                {
                    return val;
                }
            }

            return default;
        }
    }
}