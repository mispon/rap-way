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
            request.Request(GameSessionLaunchMode.NewCareer, CareerStartTemplateId.FormerGroupMember);

            GameSessionLaunchRequestData first = request.Consume();
            GameSessionLaunchRequestData second = request.Consume();

            Assert.That(first.Mode, Is.EqualTo(GameSessionLaunchMode.NewCareer));
            Assert.That(first.StartTemplateId, Is.EqualTo(CareerStartTemplateId.FormerGroupMember));
            Assert.That(second.Mode, Is.EqualTo(GameSessionLaunchMode.MainMenu));
            Assert.That(second.StartTemplateId, Is.Null);
        }

        [Test]
        public void LastRequestedMode_AndTemplate_ReturnLatestRequest_AfterConsume()
        {
            GameSessionLaunchRequest request = new();
            request.Request(GameSessionLaunchMode.NewCareer, CareerStartTemplateId.OneMemeWonder);

            _ = request.Consume();

            Assert.That(request.LastRequestedMode, Is.EqualTo(GameSessionLaunchMode.NewCareer));
            Assert.That(request.LastRequestedStartTemplateId, Is.EqualTo(CareerStartTemplateId.OneMemeWonder));
        }

        [Test]
        public void NonCareerRequest_ClearsPreviousTemplateSelection()
        {
            GameSessionLaunchRequest request = new();
            request.Request(GameSessionLaunchMode.NewCareer, CareerStartTemplateId.PrivilegedStart);
            request.Request(GameSessionLaunchMode.Continue);

            GameSessionLaunchRequestData consumed = request.Consume();

            Assert.That(consumed.Mode, Is.EqualTo(GameSessionLaunchMode.Continue));
            Assert.That(consumed.StartTemplateId, Is.Null);
        }
    }
}
