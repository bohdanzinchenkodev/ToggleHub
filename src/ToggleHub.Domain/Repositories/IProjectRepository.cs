using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Repositories;

public interface IProjectRepository : IBaseSluggedRepository<Project>
{
    Task<bool> NameExistsAsync(string name, int organizationId = 0);
    Task<IPagedList<Project>> GetAllAsync(int? organizationId = null, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<Project?> GetBySlugAsync(string slug, int organizationId);
}
