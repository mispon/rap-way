using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Character
{
    public readonly struct CharacterIdentity : IEquatable<CharacterIdentity>
    {
        public CharacterIdentity(StableId id, StableId startTemplateId)
        {
            if (!id.IsValid)
            {
                throw new ArgumentException("Character ID must be valid.", nameof(id));
            }

            if (!startTemplateId.IsValid)
            {
                throw new ArgumentException("Start template ID must be valid.", nameof(startTemplateId));
            }

            Id = id;
            StartTemplateId = startTemplateId;
        }

        public StableId Id { get; }

        public StableId StartTemplateId { get; }

        public bool Equals(CharacterIdentity other)
        {
            return Id == other.Id && StartTemplateId == other.StartTemplateId;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, StartTemplateId);
        }
    }
}
