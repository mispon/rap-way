using UnityEngine;

namespace Game.Rappers
{
    public enum FansChangeDir
    {
        None = 0,
        Decrease = 1,
        Increase = 2
    }
    
    public partial class RappersPackage
    {
        private void RandomlyChangeFans()
        {
            int minFans = _settings.Rappers.MinFans;
            int maxFans = _settings.Rappers.MaxFans;
            
            foreach (var rapperInfo in GetAll())
            {
                var dice = Random.Range(0, 3);
                if (dice == (int) FansChangeDir.None)
                {
                    continue;
                }

                var value = dice == (int) FansChangeDir.Increase ? 1 : -1;
                rapperInfo.Fans = Mathf.Clamp(rapperInfo.Fans + value, minFans, maxFans);
            }
        }
    }
}