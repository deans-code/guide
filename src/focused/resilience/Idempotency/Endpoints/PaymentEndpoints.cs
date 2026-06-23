using System.Text.Json;
using IdempotencyDemo.Data;
using IdempotencyDemo.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace IdempotencyDemo.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this WebApplication app)
    {
        app.MapPost("/payments", async (
            HttpContext http,
            PaymentRequest req,
            AppDbContext db,
            IdempotencyService idempotency,
            CancellationToken ct) =>
        {
            var key = http.Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (key is not null)
            {
                var requestHash = IdempotencyService.ComputeHash(req);
                var existing = await idempotency.FindAsync(key, ct);

                if (existing is not null)
                {
                    if (existing.RequestHash != requestHash)
                        return Results.UnprocessableEntity(new
                        {
                            error = "This idempotency key was used with a different request body."
                        });

                    // Return the cached response — payment is NOT processed again.
                    http.Response.Headers["Idempotency-Replayed"] = "true";
                    return Results.Json(
                        JsonSerializer.Deserialize<object>(existing.ResponseBody),
                        statusCode: existing.StatusCode);
                }

                // First time seeing this key — process and cache.
                var payment = await ProcessPaymentAsync(req, db, ct);
                var responseBody = JsonSerializer.Serialize(payment);
                await idempotency.StoreAsync(key, requestHash, responseBody, 200, ct);
                return Results.Ok(payment);
            }

            // No idempotency key — process without caching.
            return Results.Ok(await ProcessPaymentAsync(req, db, ct));
        })
        .WithName("CreatePayment")
        .WithSummary("Process a payment. Supply Idempotency-Key header to make it safe to retry.");

        app.MapGet("/payments/{id:guid}", async (Guid id, AppDbContext db, CancellationToken ct) =>
        {
            var payment = await db.Payments.FindAsync([id], ct);
            return payment is null ? Results.NotFound() : Results.Ok(payment);
        })
        .WithName("GetPayment")
        .WithSummary("Get a payment by ID");

        app.MapGet("/payments", async (AppDbContext db, CancellationToken ct) =>
            await db.Payments.OrderByDescending(p => p.CreatedAt).ToListAsync(ct))
        .WithName("ListPayments")
        .WithSummary("List all payments — duplicate requests with the same key appear only once");

        app.MapGet("/idempotency-keys", async (AppDbContext db, CancellationToken ct) =>
            await db.IdempotencyRecords.OrderByDescending(r => r.CreatedAt).ToListAsync(ct))
        .WithName("ListIdempotencyKeys")
        .WithSummary("Inspect stored idempotency keys and their cached responses");
    }

    private static async Task<Payment> ProcessPaymentAsync(PaymentRequest req, AppDbContext db, CancellationToken ct)
    {
        var payment = new Payment
        {
            CustomerId = req.CustomerId,
            Amount     = req.Amount,
            Currency   = req.Currency
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync(ct);
        return payment;
    }
}

public record PaymentRequest(string CustomerId, decimal Amount, string Currency);
