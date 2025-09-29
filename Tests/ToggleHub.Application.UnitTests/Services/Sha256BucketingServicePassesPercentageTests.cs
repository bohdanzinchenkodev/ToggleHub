using ToggleHub.Application.Services;

namespace ToggleHub.Application.UnitTests.Services;

public class Sha256BucketingServicePassesPercentageTests
{
    private Sha256BucketingService _bucketingService;

    [SetUp]
    public void SetUp()
    {
        _bucketingService = new Sha256BucketingService();
    }

    [Test]
    public void PassesPercentage_WithZeroPercentage_ShouldAlwaysReturnFalse()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "test-flag";
        var percentage = 0;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var stickyKey = $"user{i}";
            var result = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);
            Assert.That(result, Is.False, $"User {i} should not pass 0% percentage");
        }
    }

    [Test]
    public void PassesPercentage_With100Percentage_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "test-flag";
        var percentage = 100;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var stickyKey = $"user{i}";
            var result = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);
            Assert.That(result, Is.True, $"User {i} should pass 100% percentage");
        }
    }

    [TestCase(-10)]
    [TestCase(-1)]
    public void PassesPercentage_WithNegativePercentage_ShouldReturnFalse(int percentage)
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "test-flag";
        var stickyKey = "user123";

        // Act
        var result = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.False);
    }

    [TestCase(101)]
    [TestCase(150)]
    [TestCase(1000)]
    public void PassesPercentage_WithPercentageOver100_ShouldReturnTrue(int percentage)
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "test-flag";
        var stickyKey = "user123";

        // Act
        var result = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void PassesPercentage_ShouldRespectPercentageDistribution()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "distribution-test";
        var percentage = 25; // 25%
        const int iterations = 10000;
        var passedCount = 0;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var stickyKey = $"user{i}";
            if (_bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey))
            {
                passedCount++;
            }
        }

        // Assert
        var actualPercentage = (double)passedCount / iterations * 100;
        // Allow for ±2% variance from expected 25%
        Assert.That(actualPercentage, Is.GreaterThan(23.0), $"Actual percentage {actualPercentage:F2}% is too low");
        Assert.That(actualPercentage, Is.LessThan(27.0), $"Actual percentage {actualPercentage:F2}% is too high");
    }

    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    [TestCase(25)]
    [TestCase(50)]
    [TestCase(75)]
    [TestCase(90)]
    [TestCase(95)]
    [TestCase(99)]
    public void PassesPercentage_WithVariousPercentages_ShouldMatchExpectedDistribution(int targetPercentage)
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "percentage-test";
        const int iterations = 10000;
        var passedCount = 0;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var stickyKey = $"user{i}";
            if (_bucketingService.PassesPercentage(targetPercentage, seed, flagKey, stickyKey))
            {
                passedCount++;
            }
        }

        // Assert
        var actualPercentage = (double)passedCount / iterations * 100;
        var tolerance = Math.Max(1.0, targetPercentage * 0.1); // At least 1% tolerance, or 10% of target
        
        Assert.That(actualPercentage, Is.GreaterThan(targetPercentage - tolerance), 
            $"Actual percentage {actualPercentage:F2}% is too low for target {targetPercentage}%");
        Assert.That(actualPercentage, Is.LessThan(targetPercentage + tolerance), 
            $"Actual percentage {actualPercentage:F2}% is too high for target {targetPercentage}%");
    }

    [Test]
    public void PassesPercentage_WithSameInputs_ShouldReturnConsistentResults()
    {
        // Arrange
        var seed = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var flagKey = "consistency-test";
        var stickyKey = "user123";
        var percentage = 50;

        // Act
        var result1 = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);
        var result2 = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);
        var result3 = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);

        // Assert
        Assert.That(result2, Is.EqualTo(result1));
        Assert.That(result3, Is.EqualTo(result1));
    }

    [Test]
    public void PassesPercentage_ShouldUseGetBucketInternally()
    {
        // Arrange
        var seed = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var flagKey = "integration-test";
        var stickyKey = "user123";
        var percentage = 50;

        // Act
        var bucket = _bucketingService.GetBucket(seed, flagKey, stickyKey);
        var passes = _bucketingService.PassesPercentage(percentage, seed, flagKey, stickyKey);

        // Assert
        var expectedPasses = bucket < percentage * 100;
        Assert.That(passes, Is.EqualTo(expectedPasses));
    }
}
