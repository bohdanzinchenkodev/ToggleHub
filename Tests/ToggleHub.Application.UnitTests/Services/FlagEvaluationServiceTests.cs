using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.UnitTests.Services
{
    [TestFixture]
    public class FlagEvaluationServiceTests
    {
        private Mock<IBucketingService> _bucketingService = null!;
        private Mock<IConditionEvaluator> _conditionEvaluator = null!;
        private Mock<IFlagRepository> _flagRepository = null!;
        private Mock<IValidator<FlagEvaluationRequest>> _validator = null!;
        private Mock<IApiKeyContext> _apiKeyContext = null!;
        private Mock<ICacheManager> _cacheManager = null!;
        private Mock<IFlagEvaluationCacheKeyFactory> _cacheKeyFactory = null!;
        private FlagEvaluationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _bucketingService = new Mock<IBucketingService>();
            _conditionEvaluator = new Mock<IConditionEvaluator>();
            _flagRepository = new Mock<IFlagRepository>();
            _validator = new Mock<IValidator<FlagEvaluationRequest>>();
            _apiKeyContext = new Mock<IApiKeyContext>();
            _cacheManager = new Mock<ICacheManager>();
            _cacheKeyFactory = new Mock<IFlagEvaluationCacheKeyFactory>();

            _service = new FlagEvaluationService(
                _bucketingService.Object,
                _conditionEvaluator.Object,
                _flagRepository.Object,
                _validator.Object,
                _apiKeyContext.Object,
                _cacheManager.Object,
                _cacheKeyFactory.Object);
        }

        private static FlagEvaluationRequest MakeRequest() =>
            new FlagEvaluationRequest { UserId = "user1", FlagKey = "flagA", ConditionAttributes = new Dictionary<string, string?>() };

        private static Flag MakeFlag(bool enabled = true) =>
            new Flag
            {
                Id = 1,
                Key = "flagA",
                Enabled = enabled,
                ReturnValueType = ReturnValueType.String,
                DefaultValueOnRaw = "on",
                DefaultValueOffRaw = "off",
                RuleSets = new List<RuleSet>()
            };

        [Test]
        public void EvaluateAsync_ShouldThrow_WhenValidationFails()
        {
            var request = MakeRequest();
            _validator.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("f", "bad") }));

            Assert.ThrowsAsync<ValidationException>(() => _service.EvaluateAsync(request));
        }

        [Test]
        public void EvaluateAsync_ShouldThrow_WhenApiKeyContextMissing()
        {
            var request = MakeRequest();
            _validator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
            _apiKeyContext.Setup(x => x.GetCurrentOrgId()).Returns((int?)null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.EvaluateAsync(request));
        }

        [Test]
        public void EvaluateAsync_ShouldThrow_WhenFlagNotFound()
        {
            var request = MakeRequest();
            _validator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
            _apiKeyContext.Setup(x => x.GetCurrentOrgId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentProjectId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentEnvironmentId()).Returns(1);

            _cacheKeyFactory.Setup(f => f.CreateCacheKey(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<FlagEvaluationContext>()))
                .Returns(new CacheKey("k", 5));

            _cacheManager.Setup(c => c.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<FlagEvaluationResult>>>()))
                .Returns<CacheKey, Func<Task<FlagEvaluationResult>>>((k, acquire) => acquire());

            _flagRepository.Setup(r => r.GetFlagByKeyAsync("flagA", 1, 1)).ReturnsAsync((Flag?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.EvaluateAsync(request));
        }

        [Test]
        public async Task EvaluateAsync_ShouldReturnOff_WhenFlagDisabled()
        {
            var request = MakeRequest();
            _validator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
            _apiKeyContext.SetupAllProperties();
            _apiKeyContext.Setup(x => x.GetCurrentOrgId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentProjectId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentEnvironmentId()).Returns(1);

            _cacheKeyFactory.Setup(f => f.CreateCacheKey(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<FlagEvaluationContext>()))
                .Returns(new CacheKey("k", 5));

            _cacheManager.Setup(c => c.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<FlagEvaluationResult>>>()))
                .Returns<CacheKey, Func<Task<FlagEvaluationResult>>>((k, acquire) => acquire());

            var flag = MakeFlag(enabled: false);
            _flagRepository.Setup(r => r.GetFlagByKeyAsync("flagA", 1, 1)).ReturnsAsync(flag);

            var result = await _service.EvaluateAsync(request);

            Assert.That(result.Value, Is.EqualTo("off"));
            Assert.That(result.Reason, Is.EqualTo("flag-disabled"));
        }

        [Test]
        public async Task EvaluateAsync_ShouldReturnOn_WhenNoRuleSets()
        {
            var request = MakeRequest();
            _validator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
            _apiKeyContext.Setup(x => x.GetCurrentOrgId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentProjectId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentEnvironmentId()).Returns(1);

            _cacheKeyFactory.Setup(f => f.CreateCacheKey(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<FlagEvaluationContext>()))
                .Returns(new CacheKey("k", 5));

            _cacheManager.Setup(c => c.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<FlagEvaluationResult>>>()))
                .Returns<CacheKey, Func<Task<FlagEvaluationResult>>>((k, acquire) => acquire());

            var flag = MakeFlag(enabled: true); // no rulesets
            _flagRepository.Setup(r => r.GetFlagByKeyAsync("flagA", 1, 1)).ReturnsAsync(flag);

            var result = await _service.EvaluateAsync(request);

            Assert.That(result.Value, Is.EqualTo("on"));
            Assert.That(result.Reason, Is.EqualTo("no-ruleset-defined"));
        }

        [Test]
        public async Task EvaluateAsync_ShouldReturnRuleValue_WhenMatchesAndPassesPercentage()
        {
            var request = MakeRequest();
            _validator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
            _apiKeyContext.Setup(x => x.GetCurrentOrgId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentProjectId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentEnvironmentId()).Returns(1);

            _cacheKeyFactory.Setup(f => f.CreateCacheKey(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<FlagEvaluationContext>()))
                .Returns(new CacheKey("k", 5));

            _cacheManager.Setup(c => c.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<FlagEvaluationResult>>>()))
                .Returns<CacheKey, Func<Task<FlagEvaluationResult>>>((k, acquire) => acquire());

            var flag = MakeFlag();
            flag.RuleSets.Add(new RuleSet { Id = 10, Priority = 1, ReturnValueRaw = "rule-on" });

            _flagRepository.Setup(r => r.GetFlagByKeyAsync("flagA", 1, 1)).ReturnsAsync(flag);
            _conditionEvaluator.Setup(c => c.Matches(It.IsAny<RuleSet>(), It.IsAny<FlagEvaluationContext>())).Returns(true);
            _bucketingService.Setup(b => b.PassesPercentage(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = await _service.EvaluateAsync(request);

            Assert.That(result.Matched, Is.True);
            Assert.That(result.Value, Is.EqualTo("rule-on"));
            Assert.That(result.Reason, Is.EqualTo("matched"));
        }

        [Test]
        public async Task EvaluateAsync_ShouldReturnOff_WhenMatchesButFailsPercentage()
        {
            var request = MakeRequest();
            _validator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
            _apiKeyContext.Setup(x => x.GetCurrentOrgId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentProjectId()).Returns(1);
            _apiKeyContext.Setup(x => x.GetCurrentEnvironmentId()).Returns(1);

            _cacheKeyFactory.Setup(f => f.CreateCacheKey(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<FlagEvaluationContext>()))
                .Returns(new CacheKey("k", 5));

            _cacheManager.Setup(c => c.GetAsync(It.IsAny<CacheKey>(), It.IsAny<Func<Task<FlagEvaluationResult>>>()))
                .Returns<CacheKey, Func<Task<FlagEvaluationResult>>>((k, acquire) => acquire());

            var flag = MakeFlag();
            flag.RuleSets.Add(new RuleSet { Id = 20, Priority = 1, ReturnValueRaw = "rule-on", OffReturnValueRaw = "rule-off" });

            _flagRepository.Setup(r => r.GetFlagByKeyAsync("flagA", 1, 1)).ReturnsAsync(flag);
            _conditionEvaluator.Setup(c => c.Matches(It.IsAny<RuleSet>(), It.IsAny<FlagEvaluationContext>())).Returns(true);
            _bucketingService.Setup(b => b.PassesPercentage(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = await _service.EvaluateAsync(request);

            Assert.That(result.Matched, Is.False);
            Assert.That(result.Value, Is.EqualTo("rule-off"));
            Assert.That(result.Reason, Is.EqualTo("percentage-miss"));
        }
    }
}
