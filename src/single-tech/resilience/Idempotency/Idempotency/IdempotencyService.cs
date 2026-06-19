using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IdempotencyDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace IdempotencyDemo.Idempotency;

public class IdempotencyService(AppDbContext db)
{
    private static readonly TimeSpan KeyLifetime = TimeSpan.FromHours(24);

    public static string ComputeHash(object request)
    {
        var json = JsonSerializer.Serialize(request);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes);
    }

    public async Task<IdempotencyRecord?> FindAsync(string key, CancellationToken ct = default) =>
        await db.IdempotencyRecords.FirstOrDefaultAsync(r => r.Key == key && r.ExpiresAt > DateTime.UtcNow, ct);

    public async Task StoreAsync(string key, string requestHash, string responseBody, int statusCode, CancellationToken ct = default)
    {
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            Key          = key,
            RequestHash  = requestHash,
            ResponseBody = responseBody,
            StatusCode   = statusCode,
            ExpiresAt    = DateTime.UtcNow.Add(KeyLifetime)
        });
        await db.SaveChangesAsync(ct);
    }
}
