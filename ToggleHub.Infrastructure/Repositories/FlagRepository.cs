using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class FlagRepository : BaseRepository<Flag>, IFlagRepository
{
    public FlagRepository(ToggleHubDbContext context) : base(context)
    {
    }
}
