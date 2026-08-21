using RapWay.Domain.Activities;
using RapWay.Domain.Common;

namespace RapWay.Application.Activities
{
    public interface IActivityDefinitionLookup
    {
        bool TryGet(StableId id, out ActivityDefinition definition);
    }
}
