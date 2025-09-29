using ToggleHub.Application.Services;

namespace ToggleHub.Application.UnitTests.Services;

public class Sha256BucketingServiceGetBucketTests
{
    private Sha256BucketingService _bucketingService;

    [SetUp]
    public void SetUp()
    {
        _bucketingService = new Sha256BucketingService();
    }

    [Test]
    public void GetBucket_ShouldReturnValueInRange0To9999()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "test-flag";
        var stickyKey = "user123";

        // Act
        var result = _bucketingService.GetBucket(seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
        Assert.That(result, Is.LessThanOrEqualTo(9999));
    }

    [Test]
    public void GetBucket_WithSameInputs_ShouldReturnSameValue()
    {
        // Arrange
        var seed = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var flagKey = "test-flag";
        var stickyKey = "user123";

        // Act
        var result1 = _bucketingService.GetBucket(seed, flagKey, stickyKey);
        var result2 = _bucketingService.GetBucket(seed, flagKey, stickyKey);

        // Assert
        Assert.That(result1, Is.EqualTo(result2));
    }

    [Test]
    public void GetBucket_WithDifferentSeeds_ShouldReturnDifferentValues()
    {
        // Arrange
        var seed1 = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var seed2 = Guid.Parse("87654321-4321-4321-4321-cba987654321");
        var flagKey = "test-flag";
        var stickyKey = "user123";

        // Act
        var result1 = _bucketingService.GetBucket(seed1, flagKey, stickyKey);
        var result2 = _bucketingService.GetBucket(seed2, flagKey, stickyKey);

        // Assert
        Assert.That(result1, Is.Not.EqualTo(result2));
    }

    [Test]
    public void GetBucket_WithDifferentFlagKeys_ShouldReturnDifferentValues()
    {
        // Arrange
        var seed = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var flagKey1 = "test-flag-1";
        var flagKey2 = "test-flag-2";
        var stickyKey = "user123";

        // Act
        var result1 = _bucketingService.GetBucket(seed, flagKey1, stickyKey);
        var result2 = _bucketingService.GetBucket(seed, flagKey2, stickyKey);

        // Assert
        Assert.That(result1, Is.Not.EqualTo(result2));
    }

    [Test]
    public void GetBucket_WithDifferentStickyKeys_ShouldReturnDifferentValues()
    {
        // Arrange
        var seed = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var flagKey = "test-flag";
        var stickyKey1 = "user123";
        var stickyKey2 = "user456";

        // Act
        var result1 = _bucketingService.GetBucket(seed, flagKey, stickyKey1);
        var result2 = _bucketingService.GetBucket(seed, flagKey, stickyKey2);

        // Assert
        Assert.That(result1, Is.Not.EqualTo(result2));
    }

    [Test]
    public void GetBucket_WithEmptyStrings_ShouldHandleGracefully()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "";
        var stickyKey = "";

        // Act
        var result = _bucketingService.GetBucket(seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
        Assert.That(result, Is.LessThanOrEqualTo(9999));
    }

    [Test]
    public void GetBucket_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "flag-with-special!@#$%^&*()_+-={}[]|\\:;\"'<>?,./";
        var stickyKey = "user!@#$%^&*()_+-={}[]|\\:;\"'<>?,./";

        // Act
        var result = _bucketingService.GetBucket(seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
        Assert.That(result, Is.LessThanOrEqualTo(9999));
    }

    [Test]
    public void GetBucket_WithUnicodeCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "flag-unicode-测试-🚀-café";
        var stickyKey = "user-unicode-测试-🚀-café";

        // Act
        var result = _bucketingService.GetBucket(seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
        Assert.That(result, Is.LessThanOrEqualTo(9999));
    }

    [Test]
    public void GetBucket_WithLongStrings_ShouldHandleCorrectly()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = new string('a', 1000); // Very long flag key
        var stickyKey = new string('b', 1000); // Very long sticky key

        // Act
        var result = _bucketingService.GetBucket(seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
        Assert.That(result, Is.LessThanOrEqualTo(9999));
    }

    [Test]
    public void GetBucket_WithNullGuid_ShouldHandleCorrectly()
    {
        // Arrange
        var seed = Guid.Empty;
        var flagKey = "test-flag";
        var stickyKey = "user123";

        // Act
        var result = _bucketingService.GetBucket(seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
        Assert.That(result, Is.LessThanOrEqualTo(9999));
    }

    [Test]
    public void GetBucket_ShouldDistributeEvenly()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "distribution-test";
        var buckets = new Dictionary<int, int>();
        const int iterations = 10000;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var stickyKey = $"user{i}";
            var bucket = _bucketingService.GetBucket(seed, flagKey, stickyKey);
            var range = bucket / 1000; // 0-9 ranges
            buckets[range] = buckets.GetValueOrDefault(range, 0) + 1;
        }

        // Assert
        // Each range should have roughly 1000 values (10% of 10000)
        // Allow for some variance in distribution (±200)
        foreach (var kvp in buckets)
        {
            Assert.That(kvp.Value, Is.GreaterThan(800), $"Range {kvp.Key} has too few values: {kvp.Value}");
            Assert.That(kvp.Value, Is.LessThan(1200), $"Range {kvp.Key} has too many values: {kvp.Value}");
        }
    }
}
