using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Domain.Entities;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.Mapping;

public static class EnvironmentMapping
{
    public static EnvironmentDto ToDto(this Environment environment)
    {
        return new EnvironmentDto
        {
            Id = environment.Id,
            Type = environment.Type
        };
    }

    public static Environment ToEntity(this CreateEnvironmentDto createDto, Environment? environment = null)
    {
        environment ??= new Environment();
        environment.Type = createDto.Type ?? EnvironmentType.Dev;
        environment.ProjectId = createDto.ProjectId;
        return environment;
    }
    
    public static Environment ToEntity(this UpdateEnvironmentDto updateDto, Environment? environment = null)
    {
        environment ??= new Environment();
        environment.Id = updateDto.Id;
        if (updateDto.Type.HasValue)
        {
            environment.Type = updateDto.Type.Value;
        }
        return environment;
    }
}
