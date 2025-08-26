using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag.Update;

public class UpdateCreateOrUpdateFlagDto : BaseCreateOrUpdateFlagDto
{
    public int Id { get; set; }
    public IList<UpdateRuleSetDto> RuleSets { get; set; } = new List<UpdateRuleSetDto>();
}
