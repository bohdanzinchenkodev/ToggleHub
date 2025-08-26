namespace ToggleHub.Application.Helpers;

public static class EnumHelpers
{
    public static T? ParseEnum<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse(typeof(T), value, true, out var result))
        {
            return (T)result;
        }
        return null;
    }
}