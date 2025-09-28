using System.Globalization;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Infrastructure.Cache;

public class CacheKeyFactory
{
    public string Create(string template, params object[] parameters)
    {
        var normalized = parameters.Select(CreateCacheKeyParameter).ToArray();
        return string.Format(template, normalized);
    }

    protected virtual object CreateCacheKeyParameter(object parameter)
    {
        return parameter switch
        {
            null => "null",

            // Handle collections of IDs
            IEnumerable<int> ids => CreateIdsHash(ids),

            // Handle collections of entities
            IEnumerable<BaseEntity> entities => CreateIdsHash(entities.Select(e => e.Id)),

            // Handle a single entity
            BaseEntity entity => entity.Id,

            // Handle decimals culture-invariant
            decimal d => d.ToString(CultureInfo.InvariantCulture),

            // Default: use ToString
            _ => parameter
        };
    }

    private string CreateIdsHash(IEnumerable<int> ids)
    {
        // stable string, e.g. "1,2,3"
        var ordered = ids.OrderBy(id => id);
        return string.Join(",", ordered);
    }
}