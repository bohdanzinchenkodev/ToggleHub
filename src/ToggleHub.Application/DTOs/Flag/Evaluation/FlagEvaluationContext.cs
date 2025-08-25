namespace ToggleHub.Application.DTOs.Flag.Evaluation;

public sealed class FlagEvaluationContext
{
    public string StickyKey { get; }
    public IReadOnlyDictionary<string, string?> Attrs { get; }

    public FlagEvaluationContext(string stickyKey, IDictionary<string, string?>? attrs = null)
    {
        if (string.IsNullOrWhiteSpace(stickyKey))
            throw new ApplicationException("stickyKey is required.");

        StickyKey = stickyKey;
        Attrs = new Dictionary<string, string?>(attrs ?? new Dictionary<string, string?>(),
            StringComparer.OrdinalIgnoreCase);
    }

    // Optional typed helpers
    public bool TryGetString(string key, out string? v) => Attrs.TryGetValue(key, out v);
    public bool TryGetBool(string key, out bool v)
    {
        v = false;
        return Attrs.TryGetValue(key, out var s) && bool.TryParse(s, out v);
    }
    public bool TryGetNumber(string key, out decimal v)
    {
        v = 0;
        return Attrs.TryGetValue(key, out var s) &&
               decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                   System.Globalization.CultureInfo.InvariantCulture, out v);
    }
}