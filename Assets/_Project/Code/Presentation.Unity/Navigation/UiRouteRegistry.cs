#nullable enable

using System;
using System.Collections.Generic;

namespace RapWay.Presentation.Unity.Navigation
{
    public sealed class UiRouteRegistry
    {
        private readonly IReadOnlyDictionary<UiRouteId, UiRouteDefinition> _definitions;

        public UiRouteRegistry(IEnumerable<UiRouteDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            Dictionary<UiRouteId, UiRouteDefinition> resolvedDefinitions = new();

            foreach (UiRouteDefinition definition in definitions)
            {
                if (definition == null)
                {
                    throw new ArgumentException("Route definitions cannot contain null entries.", nameof(definitions));
                }

                if (!resolvedDefinitions.TryAdd(definition.Id, definition))
                {
                    throw new ArgumentException($"Route '{definition.Id}' is registered more than once.", nameof(definitions));
                }
            }

            if (!resolvedDefinitions.TryGetValue(UiRouteId.Home, out UiRouteDefinition? homeDefinition) ||
                homeDefinition == null ||
                homeDefinition.NavigationMode != UiNavigationMode.Home)
            {
                throw new ArgumentException("The registry must contain the Home route with Home navigation mode.", nameof(definitions));
            }

            _definitions = resolvedDefinitions;
        }

        public UiRouteDefinition Get(UiRouteId id)
        {
            if (!_definitions.TryGetValue(id, out UiRouteDefinition? definition) || definition == null)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "The route is not registered.");
            }

            return definition;
        }

        public static UiRouteRegistry CreateDefault()
        {
            return new UiRouteRegistry(new[]
            {
                new UiRouteDefinition(UiRouteId.Home, UiNavigationMode.Home, typeof(EmptyUiRouteContext), UiBackAffordance.Hidden),
                new UiRouteDefinition(UiRouteId.Map, UiNavigationMode.Push, typeof(EmptyUiRouteContext), UiBackAffordance.Hidden),
                new UiRouteDefinition(UiRouteId.Career, UiNavigationMode.Push, typeof(EmptyUiRouteContext), UiBackAffordance.Hidden),
                new UiRouteDefinition(UiRouteId.Inbox, UiNavigationMode.Push, typeof(EmptyUiRouteContext), UiBackAffordance.Hidden),
                new UiRouteDefinition(UiRouteId.ActivitySelection, UiNavigationMode.Push, typeof(EmptyUiRouteContext), UiBackAffordance.Visible),
                new UiRouteDefinition(UiRouteId.CreativeHub, UiNavigationMode.Push, typeof(EmptyUiRouteContext), UiBackAffordance.Visible),
                new UiRouteDefinition(UiRouteId.Track, UiNavigationMode.Push, typeof(EntityUiRouteContext), UiBackAffordance.Visible),
                new UiRouteDefinition(UiRouteId.TrackLyrics, UiNavigationMode.Push, typeof(EntityUiRouteContext), UiBackAffordance.Visible),
                new UiRouteDefinition(UiRouteId.ArtistProfile, UiNavigationMode.Push, typeof(EntityUiRouteContext), UiBackAffordance.Visible),
                new UiRouteDefinition(UiRouteId.NewsItem, UiNavigationMode.Push, typeof(EntityUiRouteContext), UiBackAffordance.Visible),
                new UiRouteDefinition(UiRouteId.ActivitySession, UiNavigationMode.Replace, typeof(ActivityUiRouteContext), UiBackAffordance.Hidden),
                new UiRouteDefinition(UiRouteId.ActivityResult, UiNavigationMode.Replace, typeof(ActivityUiRouteContext), UiBackAffordance.Hidden),
                new UiRouteDefinition(UiRouteId.Dialog, UiNavigationMode.Dialog, typeof(DialogUiRouteContext), UiBackAffordance.Hidden)
            });
        }
    }
}
