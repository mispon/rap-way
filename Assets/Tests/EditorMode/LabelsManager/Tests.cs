using System.Collections.Generic;
using Data;
using Models.Game;
using NUnit.Framework;

namespace Tests.EditorMode.LabelsManager
{
    public class Tests
    {
        [Test]
        public void MapScoreToPrestigeTest()
        {
            var method = TestUtils.CreateStaticMethod<Game.LabelsManager>("MapScoreToPrestige", _ => {});

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
                var prestige = Game.LabelsManager.GetLabelPrestige(label, expToLevelUp);
                Assert.AreEqual(wantPrestige, prestige);                
            }
            
        }
        
        [Test]
        public void UpdatePrestigeTest()
        {
            const int expChangeValue = 100;
            int[] expToLevelUp = {
                100, 200, 300, 400, 500, 0
            };
            
            var testCases = new UpdatePrestigeTestCase[]
            {
                new()
                {
                    name = "Dice 0 don't change label",
                    label = new LabelInfo
                    {
                        Name = "Foo",
                        Prestige = new ExpValue {Value = 1, Exp = 50}
                    },
                    dice = 0,
                    want = new ExpValue {Value = 1, Exp = 50}
                },
                new()
                {
                    name = "Decrease exp but not level",
                    label = new LabelInfo
                    {
                        Name = "Foo",
                        Prestige = new ExpValue {Value = 2, Exp = 200}
                    },
                    dice = -1,
                    want = new ExpValue {Value = 2, Exp = 100}
                },
                new()
                {
                    name = "Decrease exp and level",
                    label = new LabelInfo
                    {
                        Name = "Foo",
                        Prestige = new ExpValue {Value = 2, Exp = 50}
                    },
                    dice = -1,
                    want = new ExpValue {Value = 1, Exp = 150}
                },
                new()
                {
                    name = "Decrease exp on min level and min exp",
                    label = new LabelInfo
                    {
                        Name = "Foo",
                        Prestige = new ExpValue {Value = 0, Exp = 0}
                    },
                    dice = -1,
                    want = new ExpValue {Value = 0, Exp = 0}
                },
                new()
                {
                    name = "Increase exp but not level",
                    label = new LabelInfo
                    {
                        Name = "Foo",
                        Prestige = new ExpValue {Value = 3, Exp = 200}
                    },
                    dice = 1,
                    want = new ExpValue {Value = 3, Exp = 300}
                },
                new()
                {
                    name = "Increase exp and level",
                    label = new LabelInfo
                    {
                        Name = "Foo",
                        Prestige = new ExpValue {Value = 3, Exp = 300}
                    },
                    dice = 1,
                    want = new ExpValue {Value = 4, Exp = 0}
                },
                new()
                {
                    name = "Increase exp on max level",
                    label = new LabelInfo
                    {
                        Name = "Foo",
                        Prestige = new ExpValue {Value = 5, Exp = 0}
                    },
                    dice = 1,
                    want = new ExpValue {Value = 5, Exp = 100}
                },
            };
            
            var method = TestUtils.CreateMethod<Game.LabelsManager>(
                "UpdatePrestige", 
                manager => manager.TestSetup(expToLevelUp, expChangeValue)
            );

            foreach (var tc in testCases)
            {
                object[] args = { tc.label, tc.dice };
                method(args);
                
                Assert.AreEqual(tc.want, tc.label.Prestige);
            }
        }
    }
}