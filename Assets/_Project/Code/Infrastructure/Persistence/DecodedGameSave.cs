using System;
using RapWay.Domain.State;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class DecodedGameSave
    {
        public DecodedGameSave(GameState state, DateTime savedAtUtc)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            SavedAtUtc = savedAtUtc;
        }

        public GameState State { get; }

        public DateTime SavedAtUtc { get; }
    }
}
