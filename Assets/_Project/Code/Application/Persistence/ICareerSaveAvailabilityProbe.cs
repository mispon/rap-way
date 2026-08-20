using System.Threading;
using System.Threading.Tasks;

namespace RapWay.Application.Persistence
{
    public interface ICareerSaveAvailabilityProbe
    {
        ValueTask<bool> HasCareerSaveAsync(CancellationToken cancellationToken);
    }
}
