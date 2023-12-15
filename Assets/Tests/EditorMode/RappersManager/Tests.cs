using System.Collections.Generic;
using Data;
using NUnit.Framework;

namespace Tests.EditorMode.RappersManager
{
    public class Tests
    {
        [Test]
        public void GetRapperScoreTest()
        {
            const int maxFans = 100_000_000;
            var rapperTestCases = new Dictionary<int, int>
            {
                [0] = 0,
                [1] = 1,
                [10] = 10,
                [25] = 25,
                [50] = 50,
                [75] = 75,
                [100] = 100,
            };

            foreach (var (fans, wantScore) in rapperTestCases)
            {
                var rapper = new RapperInfo { Fans = fans };
                var score = Game.RappersManager.GetRapperScore(rapper, maxFans);
                
                Assert.AreEqual(wantScore, score);
            }
            
            var playerTestCases = new Dictionary<int, int>
            {
                [0] = 0,
                [100] = 0,
                [10_000] = 0,
                [100_000] = 0,
                [1_000_000] = 1,
                [10_000_000] = 10,
                [25_000_000] = 25,
                [50_000_000] = 50,
                [75_000_000] = 75,
                [100_000_000] = 100,
            };

            foreach (var (fans, wantScore) in playerTestCases)
            {
                var rapper = new RapperInfo { Fans = fans, IsPlayer = true };
                var score = Game.RappersManager.GetRapperScore(rapper, maxFans);
                
                Assert.AreEqual(wantScore, score);
            }
        }
    }
}