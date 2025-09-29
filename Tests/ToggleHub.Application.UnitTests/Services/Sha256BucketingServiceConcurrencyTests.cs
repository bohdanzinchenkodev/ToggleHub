using ToggleHub.Application.Services;

namespace ToggleHub.Application.UnitTests.Services;

public class Sha256BucketingServiceConcurrencyTests
{
    private Sha256BucketingService _bucketingService;

    [SetUp]
    public void SetUp()
    {
        _bucketingService = new Sha256BucketingService();
    }

    [Test]
    public void BucketingService_ShouldProduceStableResults()
    {
        // Arrange
        var testCases = new[]
        {
            (Guid.Parse("12345678-1234-1234-1234-123456789abc"), "flag1", "user1"),
            (Guid.Parse("87654321-4321-4321-4321-cba987654321"), "flag2", "user2"),
            (Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), "flag3", "user3"),
        };

        // Act & Assert
        foreach (var (seed, flagKey, stickyKey) in testCases)
        {
            var bucket1 = _bucketingService.GetBucket(seed, flagKey, stickyKey);
            var bucket2 = _bucketingService.GetBucket(seed, flagKey, stickyKey);
            
            Assert.That(bucket2, Is.EqualTo(bucket1), 
                $"Bucket should be stable for seed: {seed}, flag: {flagKey}, user: {stickyKey}");

            var passes1 = _bucketingService.PassesPercentage(50, seed, flagKey, stickyKey);
            var passes2 = _bucketingService.PassesPercentage(50, seed, flagKey, stickyKey);
            
            Assert.That(passes2, Is.EqualTo(passes1), 
                $"Percentage result should be stable for seed: {seed}, flag: {flagKey}, user: {stickyKey}");
        }
    }

    [Test]
    public async Task BucketingService_ShouldHandleConcurrentAccess()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "concurrent-test";
        const int taskCount = 100;
        const int iterationsPerTask = 10;

        // Act
        var tasks = Enumerable.Range(0, taskCount).Select(taskId =>
        {
            return Task.Run(() =>
            {
                var results = new List<(int bucket, bool passes)>();
                
                for (int i = 0; i < iterationsPerTask; i++)
                {
                    var stickyKey = $"task{taskId}-user{i}";
                    var bucket = _bucketingService.GetBucket(seed, flagKey, stickyKey);
                    var passes = _bucketingService.PassesPercentage(50, seed, flagKey, stickyKey);
                    results.Add((bucket, passes));
                }
                
                return results;
            });
        }).ToArray();

        var allResults = (await Task.WhenAll(tasks)).SelectMany(r => r).ToList();

        // Assert
        Assert.That(allResults.Count, Is.EqualTo(taskCount * iterationsPerTask));
        
        // Verify all buckets are in valid range
        foreach (var (bucket, _) in allResults)
        {
            Assert.That(bucket, Is.GreaterThanOrEqualTo(0));
            Assert.That(bucket, Is.LessThanOrEqualTo(9999));
        }

        // Verify consistency - same inputs should produce same results
        var groupedResults = allResults.GroupBy(r => $"task{allResults.IndexOf(r) / iterationsPerTask}-user{allResults.IndexOf(r) % iterationsPerTask}");
        foreach (var group in groupedResults)
        {
            var distinctBuckets = group.Select(r => r.bucket).Distinct().Count();
            var distinctPasses = group.Select(r => r.passes).Distinct().Count();
            
            Assert.That(distinctBuckets, Is.EqualTo(1), $"Same input should produce same bucket for {group.Key}");
            Assert.That(distinctPasses, Is.EqualTo(1), $"Same input should produce same percentage result for {group.Key}");
        }
    }

    [Test]
    public async Task BucketingService_ShouldMaintainConsistencyUnderLoad()
    {
        // Arrange
        var seed = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var flagKey = "load-test";
        const int concurrentUsers = 1000;

        // Act
        var tasks = Enumerable.Range(0, concurrentUsers).Select(userId =>
        {
            return Task.Run(() =>
            {
                var stickyKey = $"user{userId}";
                
                // Call multiple times to verify consistency
                var bucket1 = _bucketingService.GetBucket(seed, flagKey, stickyKey);
                var bucket2 = _bucketingService.GetBucket(seed, flagKey, stickyKey);
                var passes1 = _bucketingService.PassesPercentage(25, seed, flagKey, stickyKey);
                var passes2 = _bucketingService.PassesPercentage(25, seed, flagKey, stickyKey);
                
                return new { UserId = userId, Bucket1 = bucket1, Bucket2 = bucket2, Passes1 = passes1, Passes2 = passes2 };
            });
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        foreach (var result in results)
        {
            Assert.That(result.Bucket2, Is.EqualTo(result.Bucket1), 
                $"User {result.UserId} should get consistent bucket values");
            Assert.That(result.Passes2, Is.EqualTo(result.Passes1), 
                $"User {result.UserId} should get consistent percentage results");
            Assert.That(result.Bucket1, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.Bucket1, Is.LessThanOrEqualTo(9999));
        }

        // Verify distribution is reasonable
        var passCount = results.Count(r => r.Passes1);
        var actualPercentage = (double)passCount / concurrentUsers * 100;
        Assert.That(actualPercentage, Is.GreaterThan(20.0), "25% target should be roughly achieved");
        Assert.That(actualPercentage, Is.LessThan(30.0), "25% target should be roughly achieved");
    }

    [Test]
    public async Task BucketingService_ShouldHandleHighThroughput()
    {
        // Arrange
        var seed = Guid.NewGuid();
        var flagKey = "throughput-test";
        const int operationsPerTask = 1000;
        const int taskCount = 10;

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var tasks = Enumerable.Range(0, taskCount).Select(taskId =>
        {
            return Task.Run(() =>
            {
                var results = new List<bool>();
                for (int i = 0; i < operationsPerTask; i++)
                {
                    var stickyKey = $"task{taskId}-operation{i}";
                    var bucket = _bucketingService.GetBucket(seed, flagKey, stickyKey);
                    var passes = _bucketingService.PassesPercentage(50, seed, flagKey, stickyKey);
                    results.Add(passes);
                }
                return results;
            });
        }).ToArray();

        var allResults = (await Task.WhenAll(tasks)).SelectMany(r => r).ToList();
        stopwatch.Stop();

        // Assert
        var totalOperations = taskCount * operationsPerTask * 2; // GetBucket + PassesPercentage
        var operationsPerSecond = totalOperations / stopwatch.Elapsed.TotalSeconds;
        
        Assert.That(allResults.Count, Is.EqualTo(taskCount * operationsPerTask));
        Assert.That(operationsPerSecond, Is.GreaterThan(10000), 
            $"Should handle at least 10k ops/sec, actual: {operationsPerSecond:F0}");
        
        // Verify reasonable distribution
        var passCount = allResults.Count(r => r);
        var actualPercentage = (double)passCount / allResults.Count * 100;
        Assert.That(actualPercentage, Is.GreaterThan(45.0), "50% target should be roughly achieved");
        Assert.That(actualPercentage, Is.LessThan(55.0), "50% target should be roughly achieved");
    }
}
