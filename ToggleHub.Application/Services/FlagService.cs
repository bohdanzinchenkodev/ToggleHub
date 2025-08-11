using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class FlagService : BaseService<Flag>
{
    private readonly IFlagRepository _flagRepository;

    public FlagService(IFlagRepository flagRepository) 
        : base(flagRepository)
    {
        _flagRepository = flagRepository;
    }
}
