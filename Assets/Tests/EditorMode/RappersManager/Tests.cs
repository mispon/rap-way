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
            var rapperTestCases = new Dictionary<int, int>
            {
                [0] = 0,
                [1] = 2,
                [10] = 20,
                [25] = 50,
                [50] = 100,
                [75] = 100,
                [100] = 100
            };

            foreach (var (fans, wantScore) in rapperTestCases)
            {
                var rapper = new RapperInfo { Fans = fans };
                var score = Game.RappersManager.GetRapperScore(rapper);
                
                Assert.AreEqual(wantScore, score);
            }
            
            var playerTestCases = new Dictionary<int, int>
            {
                [0] = 0,
                [100] = 0,
                [10_000] = 0,
                [100_000] = 0,
                [1_000_000] = 2,
                [10_000_000] = 20,
                [25_000_000] = 50,
                [50_000_000] = 100,
                [75_000_000] = 100,
                [100_000_000] = 100
            };

            foreach (var (fans, wantScore) in playerTestCases)
            {
                var rapper = new RapperInfo { Fans = fans, IsPlayer = true };
                var score = Game.RappersManager.GetRapperScore(rapper);
                
                Assert.AreEqual(wantScore, score);
            }
        }
        
        [Test]
        public void MapScoreToPrestigeTest()
        {
            var testCases = new Dictionary<int, float>
            {
                [0] = 0.0f,
                [2] = 0.0f,
                [3] = 0.5f,
                [5] = 1.0f,
                [10] = 1.5f,
                [15] = 2.0f,
                [20] = 2.5f,
                [25] = 3.0f,
                [30] = 3.5f,
                [35] = 4.0f,
                [40] = 4.5f,
                [45] = 5.0f,
                [50] = 5.0f,
                [75] = 5.0f,
                [100] = 5.0f
            };

            foreach (var (fans, wantPrestige) in testCases)
            {
                var rapper = new RapperInfo { Fans = fans };
                var prestige = Game.RappersManager.GetRapperPrestige(rapper);
                
                Assert.AreEqual(wantPrestige, prestige);   
            }
        }
    }
}