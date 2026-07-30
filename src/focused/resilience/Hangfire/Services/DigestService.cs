using HangfireDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace HangfireDemo.Services;

public class DigestService(AppDbContext db, ILogger<DigestService> logger) : IDigestService
{
    public async Task GenerateDigestAsync()
    {
        var emailsCovered = await db.EmailLogs.CountAsync();

        db.DigestRuns.Add(new DigestRun
        {
            EmailsCovered = emailsCovered,
            RanAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        logger.LogInformation("Digest generated, covering {Count} emails sent so far", emailsCovered);
    }
}
