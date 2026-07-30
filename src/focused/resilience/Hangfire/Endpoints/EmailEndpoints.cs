using HangfireDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace HangfireDemo.Endpoints;

public static class EmailEndpoints
{
    public static void MapEmailEndpoints(this WebApplication app)
    {
        app.MapGet("/emails", async (AppDbContext db, CancellationToken ct) =>
            await db.EmailLogs.OrderByDescending(e => e.SentAtUtc).ToListAsync(ct))
        .WithName("ListEmails")
        .WithSummary("List emails that were actually sent — i.e. the job succeeded, including after retries");

        app.MapGet("/digests", async (AppDbContext db, CancellationToken ct) =>
            await db.DigestRuns.OrderByDescending(d => d.RanAtUtc).ToListAsync(ct))
        .WithName("ListDigestRuns")
        .WithSummary("List executions of the recurring digest job");
    }
}
