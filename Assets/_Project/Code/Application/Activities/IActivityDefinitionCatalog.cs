using System.Collections.Generic;
using RapWay.Domain.Activities;

namespace RapWay.Application.Activities
{
    public interface IActivityDefinitionCatalog : IActivityDefinitionLookup
    {
        IReadOnlyList<ActivityDefinition> Definitions { get; }
    }
}
