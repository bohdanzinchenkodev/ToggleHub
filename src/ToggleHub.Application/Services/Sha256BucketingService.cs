using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Application.Services;

public class Sha256BucketingService : IBucketingService
{
    public int GetBucket(Guid seed, string flagKey, string stickyKey)
    {
        var s = $"{seed:N}:{flagKey}:{stickyKey}";
        var bytes = Encoding.UTF8.GetBytes(s);
        var hash = SHA256.HashData(bytes);
        ulong v = BinaryPrimitives.ReadUInt64BigEndian(hash.AsSpan(0, 8));
        return (int)(v % 10000); // 0..9999
    }

    public bool PassesPercentage(int percentage, Guid seed, string flagKey, string stickyKey)
    {
        var pct = Math.Clamp(percentage, 0, 100);
        if (pct >= 100)
            return true;

        if (pct <= 0)
            return false;

        int bucket = GetBucket(seed, flagKey, stickyKey);
        return bucket < pct * 100;
    }
}