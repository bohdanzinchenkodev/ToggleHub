using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class ProjectService : BaseService<Project>
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository) 
        : base(projectRepository)
    {
        _projectRepository = projectRepository;
    }
}
