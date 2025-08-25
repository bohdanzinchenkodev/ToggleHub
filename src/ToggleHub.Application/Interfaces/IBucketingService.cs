namespace ToggleHub.Application.Interfaces;

public interface IBucketingService
{
    int GetBucket(Guid seed, string flagKey, string stickyKey); // 0..9999
    bool PassesPercentage(int percentage, Guid seed, string flagKey, string stickyKey);
}