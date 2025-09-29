using System.Text.RegularExpressions;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Services;

public class SlugGenerator : ISlugGenerator
{
    public async Task<string> GenerateAsync(
        string name,
        Func<string, Task<IEnumerable<string>>>? existingSlugsFactory = null
    ) 
    {
        string baseSlug = GenerateValidSlug(name);

        // If no factory is provided, assume no existing slugs
        var existingSlugs = existingSlugsFactory != null
            ? (await existingSlugsFactory(baseSlug)).ToArray()
            : Array.Empty<string>();

        // If base slug doesn't exist, use it
        if (!existingSlugs.Contains(baseSlug))
        {
            return baseSlug;
        }

        // Find the highest counter from existing slugs
        int maxCounter = 0;
        var counterPattern = new Regex($"^{Regex.Escape(baseSlug)}-(\\d+)$");

        foreach (var slug in existingSlugs)
        {
            var match = counterPattern.Match(slug);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int counter))
            {
                maxCounter = Math.Max(maxCounter, counter);
            }
        }

        // Return the next available slug
        return $"{baseSlug}-{maxCounter + 1}";
    }

    private static string GenerateValidSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ApplicationException("Name cannot be null or empty");

        // Convert to lowercase
        string slug = name.ToLower();

        // Replace spaces and multiple whitespace with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");

        // Remove or replace special characters, keeping only alphanumeric and hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\\-]", "");

        // Remove multiple consecutive hyphens
        slug = Regex.Replace(slug, @"-+", "-");

        // Remove leading and trailing hyphens
        slug = slug.Trim('-');

        // Ensure slug is not empty
        if (string.IsNullOrEmpty(slug))
            throw new ApplicationException("Generated slug is empty. Please provide a valid name.");

        // Limit slug length (optional)
        if (slug.Length > 50)
            slug = slug.Substring(0, 50).TrimEnd('-');

        return slug;
    }
}
