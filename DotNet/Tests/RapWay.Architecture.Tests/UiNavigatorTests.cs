using System;
using NUnit.Framework;
using RapWay.Domain.Localization;
using RapWay.Presentation.Unity.Navigation;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class UiNavigatorTests
    {
        [Test]
        public void PushAcrossSections_PreservesVisitOrderAndLocalState()
        {
            UiNavigator navigator = CreateNavigator(historyLimit: 50);
            navigator.InitializeHome();
            navigator.Navigate(UiRouteId.Career);
            TestRouteState careerState = new TestRouteState("artists-filter");
            navigator.UpdateCurrentLocalState(careerState);
            navigator.Navigate(UiRouteId.Inbox);
            navigator.Navigate(UiRouteId.ArtistProfile, new EntityUiRouteContext(UiEntityType.Artist, "artist.north-star"));

            Assert.That(navigator.Snapshot.History, Has.Count.EqualTo(4));
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.ArtistProfile));

            Assert.That(navigator.TryGoBack(), Is.True);
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.Inbox));
            Assert.That(navigator.TryGoBack(), Is.True);
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.Career));
            Assert.That(navigator.Snapshot.CurrentRoute.LocalState, Is.SameAs(careerState));
        }

        [Test]
        public void Dialog_DoesNotEnterHistoryAndBackDismissesItBeforePoppingOrigin()
        {
            UiNavigator navigator = CreateNavigator(historyLimit: 50);
            navigator.InitializeHome();
            navigator.Navigate(UiRouteId.Inbox);
            navigator.Navigate(UiRouteId.Dialog, CreateDialogContext());

            Assert.That(navigator.Snapshot.History, Has.Count.EqualTo(2));
            Assert.That(navigator.Snapshot.ActiveDialog, Is.Not.Null);
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.Inbox));

            Assert.That(navigator.TryGoBack(), Is.True);
            Assert.That(navigator.Snapshot.ActiveDialog, Is.Null);
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.Inbox));

            Assert.That(navigator.TryGoBack(), Is.True);
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.Home));
        }

        [Test]
        public void ReplaceAboveHome_DoesNotDiscardTheHomeRoot()
        {
            UiNavigator navigator = CreateNavigator(historyLimit: 50);
            navigator.InitializeHome();
            navigator.Navigate(UiRouteId.ActivitySelection);
            navigator.Navigate(UiRouteId.ActivityConfirmation, new ActivityUiRouteContext("job.courier"));
            navigator.Navigate(UiRouteId.ActivitySession, new ActivityUiRouteContext("job.courier", "session.001"));
            navigator.Navigate(UiRouteId.ActivityResult, new ActivityUiRouteContext("job.courier", "session.001", durationHours: 4));

            Assert.That(navigator.Snapshot.History, Has.Count.EqualTo(3));
            Assert.That(navigator.Snapshot.History[0].Definition.Id, Is.EqualTo(UiRouteId.Home));
            Assert.That(navigator.Snapshot.History[1].Definition.Id, Is.EqualTo(UiRouteId.ActivitySelection));
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.ActivityResult));
        }

        [Test]
        public void PushAtLimit_RemovesOldestRouteAfterHome()
        {
            UiNavigator navigator = CreateNavigator(historyLimit: 3);
            navigator.InitializeHome();
            navigator.Navigate(UiRouteId.Map);
            navigator.Navigate(UiRouteId.Career);
            navigator.Navigate(UiRouteId.Inbox);

            Assert.That(navigator.Snapshot.History, Has.Count.EqualTo(3));
            Assert.That(navigator.Snapshot.History[0].Definition.Id, Is.EqualTo(UiRouteId.Home));
            Assert.That(navigator.Snapshot.History[1].Definition.Id, Is.EqualTo(UiRouteId.Career));
            Assert.That(navigator.Snapshot.History[2].Definition.Id, Is.EqualTo(UiRouteId.Inbox));
        }

        [Test]
        public void GoHome_ClearsHistoryAndResetsTransientHomeState()
        {
            UiNavigator navigator = CreateNavigator(historyLimit: 50);
            navigator.InitializeHome();
            navigator.UpdateCurrentLocalState(new TestRouteState("old-home-state"));
            navigator.Navigate(UiRouteId.Career);

            navigator.GoHome();

            Assert.That(navigator.Snapshot.History, Has.Count.EqualTo(1));
            Assert.That(navigator.Snapshot.CurrentRoute!.Definition.Id, Is.EqualTo(UiRouteId.Home));
            Assert.That(navigator.Snapshot.CurrentRoute.LocalState, Is.SameAs(EmptyUiRouteLocalState.Instance));
        }

        [Test]
        public void Navigate_RejectsAnUnexpectedContextType()
        {
            UiNavigator navigator = CreateNavigator(historyLimit: 50);
            navigator.InitializeHome();

            Action navigateWithUnexpectedContext = () =>
                navigator.Navigate(UiRouteId.ArtistProfile, new ActivityUiRouteContext("job.courier"));
            ArgumentException? exception = Assert.Throws<ArgumentException>(navigateWithUnexpectedContext);

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception!.Message, Does.Contain(nameof(EntityUiRouteContext)));
        }

        private static UiNavigator CreateNavigator(int historyLimit)
        {
            return new UiNavigator(UiRouteRegistry.CreateDefault(), historyLimit);
        }

        private static DialogUiRouteContext CreateDialogContext()
        {
            return new DialogUiRouteContext(
                new LocalizationKey("UI.Common", "dialog_title"),
                new LocalizationKey("UI.Common", "dialog_body"),
                new[]
                {
                    new UiDialogAction(
                        "close",
                        new LocalizationKey("UI.Common", "close"),
                        isPrimary: true)
                });
        }

        private sealed class TestRouteState : IUiRouteLocalState
        {
            public TestRouteState(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }
}
