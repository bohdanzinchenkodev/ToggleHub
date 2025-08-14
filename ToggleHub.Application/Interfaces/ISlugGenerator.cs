using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Interfaces;

public interface ISlugGenerator
{
    Task<string> GenerateAsync<T>(string name) where T : BaseEntity, ISluggedEntity;
    Task<T?> GetBySlugAsync<T>(string slug) where T : BaseEntity, ISluggedEntity;
}