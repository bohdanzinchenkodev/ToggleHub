using System;
using System.Text.Json;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Validators.Flag;

public static class FlagValueTypeHelper
{
    public static bool IsBoolean(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return false;
        return v.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            v.Equals("false", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsNumber(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return false;
        return double.TryParse(v, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out _);
    }
        

    public static bool IsJson(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return false;
        try
        {
            using var _ = JsonDocument.Parse(v);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string BuildErr(string field, ReturnValueType? type) =>
        type switch
        {
            ReturnValueType.Boolean => $"{field} must be 'true' or 'false' for Boolean flags.",
            ReturnValueType.Number => $"{field} must be a valid number for Number flags.",
            ReturnValueType.Json => $"{field} must be valid JSON for Json flags.",
            ReturnValueType.String => $"{field} must be a non-empty string for String flags.",
            _ => $"{field} has invalid value."
        };
}

