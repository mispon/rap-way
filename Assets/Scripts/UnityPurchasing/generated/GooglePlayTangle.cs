// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Ft2YdfaHzH6HQUy0zFCFwdWgTcl8a0s6IfGmbcWwdlNIlh3g4iz8NS/pb+zzvkbIfzVjdW8TYr1t6BRrbln01i6K1FHJmdYqi9bAuKvIS0ts3l1+bFFaVXbaFNqrUV1dXVlcX5ipMcMHbXNof9Me3XaG1Dh8Idy68g4bAaDGJJ8+H1NyhmFvceN/eFfeXVNcbN5dVl7eXV1c0kL+hdphCUHFz6mt0fPH94LuZ5m1TIXjGXQJAjWlZgEIaksst/dv4K1xgmkln2hHO8vyw2ZlSGvGU6ack9+m8i3s3htK64StuSgGhSxP6CBhpWaklNfk89LVg9MTqItfoc1hYAcEWoltLH6s9f+G2TPd/4Feqbg9AKH930li1YOEV6KQJl9FFV5fXVxd");
        private static int[] order = new int[] { 7,8,13,4,7,13,6,8,12,11,11,12,13,13,14 };
        private static int key = 92;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
