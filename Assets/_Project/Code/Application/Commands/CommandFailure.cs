using System;
using RapWay.Domain.Common;

namespace RapWay.Application.Commands
{
    public readonly struct CommandFailure : IEquatable<CommandFailure>
    {
        public CommandFailure(StableId code)
        {
            if (!code.IsValid)
            {
                throw new ArgumentException("A command failure requires a valid stable code.", nameof(code));
            }

            Code = code;
        }

        public StableId Code { get; }

        public bool Equals(CommandFailure other)
        {
            return Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            return obj is CommandFailure other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override string ToString()
        {
            return Code.ToString();
        }
    }
}
