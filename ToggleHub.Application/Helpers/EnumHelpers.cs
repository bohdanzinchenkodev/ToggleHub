namespace ToggleHub.Application.Helpers;

public static class EnumHelpers
{
    public static T ParseEnum<T>(string value) where T : Enum
    {
        if (Enum.TryParse(typeof(T), value, true, out var result))
        {
            return (T)result;
        }
        throw new ArgumentException($"Invalid value '{value}' for enum '{typeof(T).Name}'");
    }
}