using System.Collections.Generic;
using Data;
using Game;
using Models.Game;
using NUnit.Framework;

namespace Tests.EditorMode
{
    public class LabelsManagerTests
    {
        [Test]
        public void MapScoreToPrestigeTest()
        {
            var method = TestUtils.CreateStaticMethod<LabelsManager>("MapScoreToPrestige");

            var testCases = new Dictionary<int, float>
            {
                [4] = 0.0f,
                [5] = 0.5f,
                [10] = 1.0f,
                [20] = 1.5f,
                [30] = 2.0f,
                [40] = 2.5f,
                [50] = 3.0f,
                [60] = 3.5f,
                [70] = 4.0f,
                [80] = 4.5f,
                [90] = 5.0f,
            };

            foreach (var (score, wantPrestige) in testCases)
            {
                object[] args = {score};
                var prestige = method(args);
                
                Assert.AreEqual(wantPrestige, prestige);   
            }
        }

        [Test]
        public void CalcRapperScoreTest()
        {
            var method = TestUtils.CreateStaticMethod<LabelsManager>("CalcRapperScore");

            const int maxFans = 100_000_000;
            var testCases = new Dictionary<int, int>
            {
                [100] = 0,
                [1000] = 0,
                [10_000] = 0,
                [100_000] = 0,
                [1_000_000] = 1,
                [10_000_000] = 10,
                [25_000_000] = 25,
                [50_000_000] = 50,
                [75_000_000] = 75,
                [100_000_000] = 100,
            };

            foreach (var (fans, wantScore) in testCases)
            {
                object[] args = {fans, maxFans};
                var score = method(args);
                
                Assert.AreEqual(wantScore, score);
            }
        }

        [Test]
        public void GetLabelPrestigeTest()
        {
            int[] expToLevelUp = {
                100, 200, 300, 400, 500, 0
            };

            var testCases = new Dictionary<ExpValue, float>
            {
                [new ExpValue{Value = 0, Exp = 49}] = 0.0f,
                [new ExpValue{Value = 0, Exp = 51}] = 0.5f,
                [new ExpValue{Value = 1, Exp = 99}] = 1.0f,
                [new ExpValue{Value = 1, Exp = 101}] = 1.5f,
                [new ExpValue{Value = 2, Exp = 149}] = 2.0f,
                [new ExpValue{Value = 2, Exp = 151}] = 2.5f,
                [new ExpValue{Value = 3, Exp = 199}] = 3.0f,
                [new ExpValue{Value = 3, Exp = 201}] = 3.5f,
                [new ExpValue{Value = 4, Exp = 249}] = 4.0f,
                [new ExpValue{Value = 4, Exp = 251}] = 4.5f,
                [new ExpValue{Value = 5, Exp = 0}] = 5.0f,
                [new ExpValue{Value = 5, Exp = 999}] = 5.0f
            };

            foreach (var (prestigeExp, wantPrestige) in testCases)
            {
                var label = new LabelInfo { Prestige = prestigeExp };
                var prestige = LabelsManager.GetLabelPrestige(label, expToLevelUp);
                Assert.AreEqual(wantPrestige, prestige);                
            }
            
        }
    }
}