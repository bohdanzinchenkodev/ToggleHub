using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Infrastructure.Repositories;

public class EnvironmentRepository : BaseRepository<Environment>, IEnvironmentRepository
{
    public EnvironmentRepository(ToggleHubDbContext context) : base(context)
    {
    }
}
