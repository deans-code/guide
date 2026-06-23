using SagaDemo.Data;
using SagaDemo.Services;

namespace SagaDemo.Saga;

public class OrderFulfillmentSaga(
    InventoryService inventory,
    PaymentService payment,
    ShippingService shipping,
    AppDbContext db)
{
    public async Task<SagaInstance> ExecuteAsync(PlaceOrderRequest request, CancellationToken ct = default)
    {
        var saga = new SagaInstance
        {
            OrderId = Guid.NewGuid().ToString(),
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Amount = request.Amount
        };
        db.Sagas.Add(saga);
        await db.SaveChangesAsync(ct);

        var steps = new StepDefinition[]
        {
            new("ReserveInventory",
                () => inventory.ReserveAsync(saga.ProductId, saga.Quantity),
                () => inventory.ReleaseAsync(saga.ProductId, saga.Quantity)),
            new("ProcessPayment",
                () => payment.ChargeAsync(saga.OrderId, saga.Amount),
                () => payment.RefundAsync(saga.OrderId)),
            new("ScheduleShipment",
                () => shipping.ScheduleAsync(saga.OrderId, saga.ProductId, saga.Quantity),
                () => shipping.CancelAsync(saga.OrderId))
        };

        var completed = new Stack<(StepDefinition Step, SagaStepRecord Record)>();

        foreach (var step in steps)
        {
            var record = new SagaStepRecord { SagaId = saga.Id, Name = step.Name };
            saga.Steps.Add(record);
            await db.SaveChangesAsync(ct);

            try
            {
                await step.Execute();
                record.Status = "Succeeded";
                record.CompletedAt = DateTime.UtcNow;
                completed.Push((step, record));
                await db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                record.Status = "Failed";
                record.Error = ex.Message;
                record.CompletedAt = DateTime.UtcNow;
                saga.Status = "Compensating";
                await db.SaveChangesAsync(ct);

                foreach (var (completedStep, completedRecord) in completed)
                {
                    completedRecord.Status = "Compensating";
                    await db.SaveChangesAsync(ct);
                    await completedStep.Compensate();
                    completedRecord.Status = "Compensated";
                    completedRecord.CompletedAt = DateTime.UtcNow;
                    await db.SaveChangesAsync(ct);
                }

                saga.Status = "Compensated";
                saga.CompletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
                return saga;
            }
        }

        saga.Status = "Completed";
        saga.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return saga;
    }
}

public record PlaceOrderRequest(string ProductId, int Quantity, decimal Amount);

file record StepDefinition(string Name, Func<Task> Execute, Func<Task> Compensate);
