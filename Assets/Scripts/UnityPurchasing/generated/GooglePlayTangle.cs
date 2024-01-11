// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("nuISKxq/vJGyH4p/RUoGfyv0NQcHhIqFtQeEj4cHhISFC5snXAO40JgcFnB0CCoeLls3vkBslVw6wK3QpbKS4/gof7Qcaa+KkU/EOTv1JewqCwxaCspxUoZ4FLi53t2DULT1pyvXwth5H/1G58aKq1+4tqg6pqGO2+x8v9jRs5L1bi62OXSoW7D8RrFBcOga3rSqsaYKxwSvXw3hpfgFY88EQawvXhWnXpiVbRWJXBgMeZQQwpMyXXRg8d9c9ZYx+bh8v31NDj23gC0P91MNiBBAD/NSDxlhchGSkvYwtjUqZ58Rpuy6rLbKu2S0Mc2ydSwmXwDqBCZYh3Bh5Nl4JAaQuwy1B4SntYiDjK8DzQNyiISEhICFhlpdjntJ/4aczIeGhIWE");
        private static int[] order = new int[] { 9,1,12,8,5,6,11,12,12,12,11,13,13,13,14 };
        private static int key = 133;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
