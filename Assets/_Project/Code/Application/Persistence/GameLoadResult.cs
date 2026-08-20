using System;
using RapWay.Domain.State;

namespace RapWay.Application.Persistence
{
    public sealed class GameLoadResult
    {
        private GameLoadResult(
            GameState? state,
            DateTime savedAtUtc,
            GameLoadSource source,
            GameLoadFailure failure,
            string detail)
        {
            State = state;
            SavedAtUtc = savedAtUtc;
            Source = source;
            Failure = failure;
            Detail = detail ?? string.Empty;
        }

        public bool IsSuccess => State != null;

        public GameState? State { get; }

        public DateTime SavedAtUtc { get; }

        public GameLoadSource Source { get; }

        public GameLoadFailure Failure { get; }

        public string Detail { get; }

        public static GameLoadResult Loaded(GameState state, DateTime savedAtUtc, GameLoadSource source)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (source == GameLoadSource.None)
            {
                throw new ArgumentOutOfRangeException(nameof(source));
            }

            return new GameLoadResult(state, savedAtUtc, source, GameLoadFailure.None, string.Empty);
        }

        public static GameLoadResult Failed(GameLoadFailure failure, string detail)
        {
            if (failure == GameLoadFailure.None)
            {
                throw new ArgumentOutOfRangeException(nameof(failure));
            }

            return new GameLoadResult(null, default, GameLoadSource.None, failure, detail);
        }
    }
}
