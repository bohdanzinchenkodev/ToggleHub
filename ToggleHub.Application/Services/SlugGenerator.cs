using System.Text.RegularExpressions;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class SlugGenerator : ISlugGenerator
{
    private readonly ISluggedRepository _repository;

    public SlugGenerator(ISluggedRepository repository)
    {
        _repository = repository;
    }

    public async Task<T?> GetBySlugAsync<T>(string slug) where T : BaseEntity, ISluggedEntity
    {
        ArgumentNullException.ThrowIfNull(slug);
        
        // Validate slug format
        if (!Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$"))
            throw new ArgumentException("Invalid slug format.", nameof(slug));

        // Fetch the entity by slug
        return await _repository.GetBySlugAsync<T>(slug);
    }
    public async Task<string> GenerateAsync<T>(string name) where T : BaseEntity, ISluggedEntity
    {
        string baseSlug = GenerateValidSlug(name);
        
        // Get all existing slugs that match the pattern
        var existingSlugs = (await _repository.GetSlugsByPatternAsync<T>(baseSlug)).ToArray();
        
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
        ArgumentNullException.ThrowIfNull(name);

        // Convert to lowercase
        string slug = name.ToLower();

        // Replace spaces and multiple whitespace with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");

        // Remove or replace special characters, keeping only alphanumeric and hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

        // Remove multiple consecutive hyphens
        slug = Regex.Replace(slug, @"-+", "-");

        // Remove leading and trailing hyphens
        slug = slug.Trim('-');

        // Ensure slug is not empty
        if (string.IsNullOrEmpty(slug))
            throw new InvalidOperationException("Generated slug is empty. Please provide a valid name.");

        // Limit slug length (optional)
        if (slug.Length > 50)
            slug = slug.Substring(0, 50).TrimEnd('-');

        return slug;
    }
}