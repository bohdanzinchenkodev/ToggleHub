using System.Reflection;
using ToggleHub.Domain.Attributes;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Helpers;

public static class OperatorTypeHelper
{
    private static readonly Dictionary<OperatorType, RuleFieldType[]> _validFieldTypesCache = new();
    private static readonly Dictionary<RuleFieldType, OperatorType[]> _validOperatorsCache = new();
    
    static OperatorTypeHelper()
    {
        foreach (OperatorType operatorType in Enum.GetValues<OperatorType>())
        {
            _validFieldTypesCache[operatorType] = GetValidRuleFieldTypes(operatorType);
        }
        
        // Build reverse cache for field type -> operators
        foreach (RuleFieldType fieldType in Enum.GetValues<RuleFieldType>())
        {
            var validOperators = Enum.GetValues<OperatorType>()
                .Where(op => IsValidFieldType(op, fieldType))
                .ToArray();
            _validOperatorsCache[fieldType] = validOperators;
        }
    }
    
    public static RuleFieldType[] GetValidFieldTypes(OperatorType operatorType)
    {
        return _validFieldTypesCache[operatorType];
    }
    
    public static bool IsValidFieldType(OperatorType operatorType, RuleFieldType fieldType)
    {
        return _validFieldTypesCache[operatorType].Contains(fieldType);
    }
    
    /// <summary>
    /// Gets all valid operators for a specific field type
    /// </summary>
    /// <param name="fieldType">The field type to get valid operators for</param>
    /// <returns>Array of valid operators for the field type</returns>
    public static OperatorType[] GetValidOperators(RuleFieldType fieldType)
    {
        return _validOperatorsCache[fieldType];
    }
    
    /// <summary>
    /// Checks if an operator is valid for a specific field type (same as IsValidFieldType but with reversed parameters for clarity)
    /// </summary>
    /// <param name="fieldType">The field type to check</param>
    /// <param name="operatorType">The operator to validate</param>
    /// <returns>True if the operator is valid for the field type</returns>
    public static bool IsValidOperator(RuleFieldType fieldType, OperatorType operatorType)
    {
        return IsValidFieldType(operatorType, fieldType);
    }
    
    private static RuleFieldType[] GetValidRuleFieldTypes(OperatorType operatorType)
    {
        var type = typeof(OperatorType);
        var memberInfo = type.GetField(operatorType.ToString());
        
        if (memberInfo != null)
        {
            var attribute = memberInfo.GetCustomAttribute<ValidRuleFieldTypesAttribute>();
            if (attribute != null)
            {
                return attribute.ValidFieldTypes;
            }
        }
        
        return [];
    }
}
