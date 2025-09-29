using System.Text.RegularExpressions;
using ToggleHub.Application.Services;

namespace ToggleHub.Application.UnitTests.Services;

public class ApiKeyGeneratorTests
{
    private ApiKeyGenerator _apiKeyGenerator;

    [SetUp]
    public void SetUp()
    {
        _apiKeyGenerator = new ApiKeyGenerator();
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldReturnNonEmptyString()
    {
        // Act
        var result = await _apiKeyGenerator.GenerateKeyAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldStartWithApiPrefix()
    {
        // Act
        var result = await _apiKeyGenerator.GenerateKeyAsync();

        // Assert
        Assert.That(result, Does.StartWith("api_"));
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldNotContainUnsafeCharacters()
    {
        // Act
        var result = await _apiKeyGenerator.GenerateKeyAsync();

        // Assert
        Assert.That(result, Does.Not.Contain("+"));
        Assert.That(result, Does.Not.Contain("/"));
        Assert.That(result, Does.Not.Contain("="));
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldOnlyContainSafeCharacters()
    {
        // Act
        var result = await _apiKeyGenerator.GenerateKeyAsync();

        // Assert
        // Should only contain alphanumeric characters and underscore
        var pattern = @"^api_[A-Za-z0-9]+$";
        Assert.That(Regex.IsMatch(result, pattern), Is.True, 
            $"API key '{result}' contains invalid characters. Should only contain alphanumeric characters and underscore after 'api_' prefix.");
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldGenerateUniqueKeys()
    {
        // Arrange
        var keys = new HashSet<string>();
        const int numberOfKeys = 100;

        // Act
        for (int i = 0; i < numberOfKeys; i++)
        {
            var key = await _apiKeyGenerator.GenerateKeyAsync();
            keys.Add(key);
        }

        // Assert
        Assert.That(keys.Count, Is.EqualTo(numberOfKeys), 
            "All generated keys should be unique");
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldGenerateRandomKeys()
    {
        // Arrange
        const int numberOfKeys = 10;
        var keys = new List<string>();

        // Act
        for (int i = 0; i < numberOfKeys; i++)
        {
            var key = await _apiKeyGenerator.GenerateKeyAsync();
            keys.Add(key);
        }

        // Assert
        // Check that not all keys are the same (highly unlikely with proper randomness)
        var distinctKeys = keys.Distinct().Count();
        Assert.That(distinctKeys, Is.EqualTo(numberOfKeys), 
            "Keys should be randomly generated and all different");
    }

    [Test]
    public async Task GenerateKeyAsync_MultipleCallsShouldReturnDifferentValues()
    {
        // Act
        var key1 = await _apiKeyGenerator.GenerateKeyAsync();
        var key2 = await _apiKeyGenerator.GenerateKeyAsync();
        var key3 = await _apiKeyGenerator.GenerateKeyAsync();

        // Assert
        Assert.That(key1, Is.Not.EqualTo(key2));
        Assert.That(key2, Is.Not.EqualTo(key3));
        Assert.That(key1, Is.Not.EqualTo(key3));
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldHandleConcurrentCalls()
    {
        // Arrange
        const int numberOfTasks = 10;
        var tasks = new List<Task<string>>();

        // Act
        for (int i = 0; i < numberOfTasks; i++)
        {
            tasks.Add(_apiKeyGenerator.GenerateKeyAsync());
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.That(results.Length, Is.EqualTo(numberOfTasks));
        Assert.That(results.Distinct().Count(), Is.EqualTo(numberOfTasks), 
            "All concurrent calls should generate unique keys");
        
        foreach (var result in results)
        {
            Assert.That(result, Does.StartWith("api_"));
        }
    }

    [Test]
    public async Task GenerateKeyAsync_ShouldReturnValidBase64DerivativeString()
    {
        // Act
        var result = await _apiKeyGenerator.GenerateKeyAsync();

        // Assert
        var keyPart = result.Substring(4); // Remove "api_" prefix
        
        // The key should be a valid Base64 string with padding and unsafe characters removed
        // Let's verify it contains only valid Base64 characters (excluding the removed ones)
        var validBase64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        foreach (var c in keyPart)
        {
            Assert.That(validBase64Chars, Does.Contain(c), 
                $"Character '{c}' is not a valid Base64 character");
        }
    }

    [Test]
    [Repeat(5)]
    public async Task GenerateKeyAsync_RepeatedTest_ShouldConsistentlyMeetRequirements()
    {
        // Act
        var result = await _apiKeyGenerator.GenerateKeyAsync();

        // Assert - All basic requirements should be met consistently
        Assert.That(result, Does.StartWith("api_"));
        Assert.That(result, Does.Not.Contain("+"));
        Assert.That(result, Does.Not.Contain("/"));
        Assert.That(result, Does.Not.Contain("="));
        Assert.That(Regex.IsMatch(result, @"^api_[A-Za-z0-9]+$"), Is.True);
    }

    [Test]
    public async Task GenerateKeyAsync_LargeScale_ShouldMaintainUniqueness()
    {
        // Arrange
        const int largeNumberOfKeys = 1000;
        var keys = new HashSet<string>();

        // Act
        var tasks = Enumerable.Range(0, largeNumberOfKeys)
            .Select(async _ => await _apiKeyGenerator.GenerateKeyAsync())
            .ToArray();

        var results = await Task.WhenAll(tasks);

        foreach (var key in results)
        {
            keys.Add(key);
        }

        // Assert
        Assert.That(keys.Count, Is.EqualTo(largeNumberOfKeys), 
            $"All {largeNumberOfKeys} generated keys should be unique");
    }
}
