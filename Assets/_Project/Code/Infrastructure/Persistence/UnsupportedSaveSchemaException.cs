using System;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class UnsupportedSaveSchemaException : Exception
    {
        public UnsupportedSaveSchemaException(int schemaVersion)
            : base($"Save schema version {schemaVersion} is not supported.")
        {
            SchemaVersion = schemaVersion;
        }

        public int SchemaVersion { get; }
    }
}
