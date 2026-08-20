using Newtonsoft.Json.Linq;

namespace RapWay.Infrastructure.Persistence.Migrations
{
    public interface ISaveMigration
    {
        int FromVersion { get; }

        JObject Migrate(JObject payload);
    }
}
