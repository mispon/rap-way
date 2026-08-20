using NUnit.Framework;
using RapWay.Application.Session;

namespace RapWay.Architecture.Tests
{
    public sealed class GameSessionLaunchRequestTests
    {
        [Test]
        public void Consume_ReturnsRequestedMode_AndResetsToMainMenu()
        {
            GameSessionLaunchRequest request = new();
            request.Request(GameSessionLaunchMode.NewCareer);

            GameSessionLaunchMode first = request.Consume();
            GameSessionLaunchMode second = request.Consume();

            Assert.That(first, Is.EqualTo(GameSessionLaunchMode.NewCareer));
            Assert.That(second, Is.EqualTo(GameSessionLaunchMode.MainMenu));
        }

        [Test]
        public void LastRequestedMode_ReturnsLatestRequestedMode_AfterConsume()
        {
            GameSessionLaunchRequest request = new();
            request.Request(GameSessionLaunchMode.Continue);

            _ = request.Consume();

            Assert.That(request.LastRequestedMode, Is.EqualTo(GameSessionLaunchMode.Continue));
        }
    }
}
