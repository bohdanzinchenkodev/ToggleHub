using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Services;

public class ConditionEvaluator : IConditionEvaluator
{
    public bool Matches(RuleSet ruleSet, FlagEvaluationContext context)
    {
        foreach (var cond in ruleSet.Conditions)
        {
            if (!Matches(cond, context)) return false;
        }

        return true;
    }

    private static bool Matches(RuleCondition ruleCondition, FlagEvaluationContext context)
    {
        var comp = StringComparison.OrdinalIgnoreCase;

        switch (ruleCondition.FieldType)
        {
            case RuleFieldType.String:
            {
                if (!context.TryGetString(ruleCondition.Field, out var actual))
                    return false;

                var expected = ruleCondition.ValueString ?? string.Empty;

                return ruleCondition.Operator switch
                {
                    OperatorType.Equals => string.Equals(actual, expected, comp),
                    OperatorType.NotEquals => !string.Equals(actual, expected, comp),
                    OperatorType.Contains => (actual ?? "").IndexOf(expected, comp) >= 0,
                    OperatorType.StartsWith => (actual ?? "").StartsWith(expected, comp),
                    OperatorType.EndsWith => (actual ?? "").EndsWith(expected, comp),
                    _ => false
                };
            }

            case RuleFieldType.Number:
            {
                if (!context.TryGetNumber(ruleCondition.Field, out var actual))
                    return false;

                var expected = ruleCondition.ValueNumber ?? 0m;

                return ruleCondition.Operator switch
                {
                    OperatorType.Equals => actual == expected,
                    OperatorType.NotEquals => actual != expected,
                    OperatorType.GreaterThan => actual > expected,
                    OperatorType.LessThan => actual < expected,
                    _ => false
                };
            }

            case RuleFieldType.Boolean:
            {
                if (!context.TryGetBool(ruleCondition.Field, out var actual))
                    return false;

                var expected = ruleCondition.ValueBoolean ?? false;

                return ruleCondition.Operator switch
                {
                    OperatorType.Equals => actual == expected,
                    OperatorType.NotEquals => actual != expected,
                    _ => false
                };
            }

            case RuleFieldType.List:
            {
                if (!context.TryGetString(ruleCondition.Field, out var actual)) return false;

                var itemsS = ruleCondition.Items
                    .Select(i => i.ValueString)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var itemsN = ruleCondition.Items
                    .Where(i => i.ValueNumber.HasValue)
                    .Select(i => i.ValueNumber!.Value)
                    .ToHashSet();

                bool contains =
                    (!string.IsNullOrWhiteSpace(actual) && itemsS.Contains(actual)) ||
                    (decimal.TryParse(actual, out var num) && itemsN.Contains(num));

                return ruleCondition.Operator switch
                {
                    OperatorType.In => contains,
                    OperatorType.NotIn => !contains,
                    _ => false
                };
            }

            default:
                return false;
        }
    }
}