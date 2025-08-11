using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.Services;

public class EnvironmentService : BaseService<Environment>
{
    private readonly IEnvironmentRepository _environmentRepository;

    public EnvironmentService(IEnvironmentRepository environmentRepository) 
        : base(environmentRepository)
    {
        _environmentRepository = environmentRepository;
    }
}
