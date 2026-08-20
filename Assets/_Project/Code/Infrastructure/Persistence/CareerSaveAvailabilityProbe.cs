using System;
using System.Threading;
using System.Threading.Tasks;
using RapWay.Application.Persistence;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class CareerSaveAvailabilityProbe : ICareerSaveAvailabilityProbe
    {
        private readonly IGameSaveStore _saveStore;

        public CareerSaveAvailabilityProbe(IGameSaveStore saveStore)
        {
            _saveStore = saveStore ?? throw new ArgumentNullException(nameof(saveStore));
        }

        public async ValueTask<bool> HasCareerSaveAsync(CancellationToken cancellationToken)
        {
            GameLoadResult loadResult = await _saveStore.LoadAsync(cancellationToken).ConfigureAwait(false);
            return loadResult.IsSuccess;
        }
    }
}
