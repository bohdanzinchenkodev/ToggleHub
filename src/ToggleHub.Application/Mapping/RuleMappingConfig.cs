using Mapster;
using Microsoft.VisualBasic.FileIO;
using ToggleHub.Application.DTOs.Flag.Create;
using ToggleHub.Application.DTOs.Flag.Update;
using ToggleHub.Application.Helpers;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public class RuleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        /*config.NewConfig<CreateRuleSetDto, RuleSet>()
            .Map(dest => dest.ReturnValueType, src => EnumHelpers.ParseEnum<ReturnValueType>(src.ReturnValueType));
        
        config.NewConfig<CreateRuleConditionDto, RuleCondition>()
            .Map(dest => dest.FieldType, src => EnumHelpers.ParseEnum<RuleFieldType>(src.FieldType))
            .Map(dest => dest.Operator, src => EnumHelpers.ParseEnum<OperatorType>(src.Operator));
        
        config.NewConfig<UpdateRuleSetDto, RuleSet>()
            .Map(dest => dest.ReturnValueType, src => EnumHelpers.ParseEnum<ReturnValueType>(src.ReturnValueType));

        config.NewConfig<UpdateRuleConditionDto, RuleCondition>()
            .Map(dest => dest.FieldType, src => EnumHelpers.ParseEnum<FieldType>(src.FieldType))
            .Map(dest => dest.Operator, src => EnumHelpers.ParseEnum<OperatorType>(src.Operator));*/
        
    }
}