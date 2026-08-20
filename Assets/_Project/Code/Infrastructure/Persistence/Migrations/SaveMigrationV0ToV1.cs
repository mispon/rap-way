using System;
using Newtonsoft.Json.Linq;

namespace RapWay.Infrastructure.Persistence.Migrations
{
    public sealed class SaveMigrationV0ToV1 : ISaveMigration
    {
        public int FromVersion => 0;

        public JObject Migrate(JObject payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            JObject migrated = (JObject)payload.DeepClone();
            JObject state = migrated["state"] as JObject ??
                            throw new FormatException("Schema v0 save has no state object.");
            state.AddFirst(new JProperty("revision", 0));
            migrated["schemaVersion"] = 1;
            return migrated;
        }
    }
}
