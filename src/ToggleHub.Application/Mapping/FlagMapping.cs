using ToggleHub.Application.DTOs.Flag;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class FlagMapping
{
    public static FlagDto ToDto(this Flag flag)
    {
        return new FlagDto
        {
            Id = flag.Id,
            ProjectId = flag.ProjectId,
            EnvironmentId = flag.EnvironmentId,
            Key = flag.Key,
            Description = flag.Description,
            Enabled = flag.Enabled,
            UpdatedAt = flag.UpdatedAt,
            ReturnValueType = flag.ReturnValueType,
            DefaultValueOnRaw = flag.DefaultValueOnRaw,
            DefaultValueOffRaw = flag.DefaultValueOffRaw,
            RuleSets = flag.RuleSets.Select(rs => rs.ToDto()).ToList()
        };
    }

    public static Flag ToEntity(this CreateFlagDto createDto, Flag? flag = null)
    {
        flag ??= new Flag();
        flag.ProjectId = createDto.ProjectId;
        flag.EnvironmentId = createDto.EnvironmentId;
        flag.Key = createDto.Key;
        flag.Description = createDto.Description;
        flag.Enabled = createDto.Enabled;
        flag.ReturnValueType = createDto.ReturnValueType ?? ReturnValueType.Boolean;
        flag.DefaultValueOnRaw = createDto.DefaultValueOnRaw;
        flag.DefaultValueOffRaw = createDto.DefaultValueOffRaw;
        flag.RuleSets = createDto.RuleSets.Select(rs => rs.ToEntity()).ToList();
        return flag;
    }
    
    public static void UpdateEntity(this UpdateFlagDto updateDto, Flag flag)
    {
        flag.Key = updateDto.Key;
        flag.Description = updateDto.Description;
        flag.Enabled = updateDto.Enabled;
        flag.DefaultValueOnRaw = updateDto.DefaultValueOnRaw;
        flag.DefaultValueOffRaw = updateDto.DefaultValueOffRaw;
    }
}
