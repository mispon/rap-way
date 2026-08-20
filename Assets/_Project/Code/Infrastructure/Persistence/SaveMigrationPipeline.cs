using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RapWay.Infrastructure.Persistence.Migrations;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class SaveMigrationPipeline
    {
        private readonly Dictionary<int, ISaveMigration> _migrations;

        public SaveMigrationPipeline()
            : this(new ISaveMigration[] { new SaveMigrationV0ToV1() })
        {
        }

        public SaveMigrationPipeline(IReadOnlyList<ISaveMigration> migrations)
        {
            if (migrations == null)
            {
                throw new ArgumentNullException(nameof(migrations));
            }

            _migrations = new Dictionary<int, ISaveMigration>(migrations.Count);
            for (int index = 0; index < migrations.Count; index++)
            {
                ISaveMigration migration = migrations[index] ??
                                           throw new ArgumentException("Migrations cannot contain null.", nameof(migrations));
                if (!_migrations.TryAdd(migration.FromVersion, migration))
                {
                    throw new ArgumentException($"Duplicate migration from schema {migration.FromVersion}.", nameof(migrations));
                }
            }
        }

        public JObject MigrateToCurrent(JObject payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            int version = ReadSchemaVersion(payload);
            if (version > GameStateSnapshotMapper.CurrentSchemaVersion || version < 0)
            {
                throw new UnsupportedSaveSchemaException(version);
            }

            JObject current = (JObject)payload.DeepClone();
            while (version < GameStateSnapshotMapper.CurrentSchemaVersion)
            {
                if (!_migrations.TryGetValue(version, out ISaveMigration migration))
                {
                    throw new UnsupportedSaveSchemaException(version);
                }

                current = migration.Migrate(current) ??
                          throw new InvalidOperationException($"Migration from schema {version} returned no payload.");
                int migratedVersion = ReadSchemaVersion(current);
                if (migratedVersion != version + 1)
                {
                    throw new InvalidOperationException($"Migration from schema {version} did not advance exactly one version.");
                }

                version = migratedVersion;
            }

            return current;
        }

        private static int ReadSchemaVersion(JObject payload)
        {
            JToken token = payload["schemaVersion"] ?? throw new FormatException("Save payload has no schemaVersion.");
            if (token.Type != JTokenType.Integer)
            {
                throw new FormatException("Save schemaVersion must be an integer.");
            }

            return token.Value<int>();
        }
    }
}
