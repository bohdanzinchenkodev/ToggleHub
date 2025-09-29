using Microsoft.Extensions.Caching.Memory;
using ToggleHub.Application;
using ToggleHub.Infrastructure.Cache;

namespace ToggleHub.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class CacheManagerTests
    {
        private InMemoryCacheManager _manager = null!;
        private InMemoryCacheKeyRegistry _registry = null!;
        private IMemoryCache _memoryCache = null!;

        [SetUp]
        public void Setup()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _registry = new InMemoryCacheKeyRegistry();
            _manager = new InMemoryCacheManager(_memoryCache, _registry);
        }
        
        [TearDown]
        public void Teardown()
        {
            _memoryCache.Dispose();
        }

        [Test]
        public async Task GetAsync_ShouldCallAcquireOnce_WhenCalledConcurrently()
        {
            var key = new CacheKey("test:1", 5);
            int callCount = 0;

            async Task<string> Acquire()
            {
                Interlocked.Increment(ref callCount);
                await Task.Delay(50); // simulate slow fetch
                return "value";
            }

            var tasks = Enumerable.Range(0, 5)
                .Select(_ => _manager.GetAsync(key, Acquire))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            Assert.That(results.All(r => r == "value"), Is.True);
            Assert.That(callCount, Is.EqualTo(1)); // acquire called only once
        }

        [Test]
        public async Task SetAsync_ShouldStoreAndReturnValue()
        {
            var key = new CacheKey("test:2", 5);
            await _manager.SetAsync(key, "hello");

            var result = await _manager.GetAsync(key, () => Task.FromResult("other"));

            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public async Task GetAsync_ShouldRemoveEntry_WhenAcquireReturnsNull()
        {
            var key = new CacheKey("test:3", 5);

            var result = await _manager.GetAsync<string?>(key, () => Task.FromResult<string?>(null));

            Assert.That(result, Is.Null);
            Assert.That(_registry.GetKeys(), Does.Not.Contain("test:3"));
        }

        [Test]
        public async Task Remove_ShouldRemoveFromCacheAndRegistry()
        {
            var key = new CacheKey("test:4", 5);
            await _manager.SetAsync(key, "bye");

            await _manager.RemoveAsync(key.Key);

            var allKeys = _registry.GetKeys().ToList();
            Assert.That(allKeys, Does.Not.Contain("test:4"));
        }
    }
}
