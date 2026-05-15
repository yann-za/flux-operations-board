using FluxOperations.Domain.Entities;
using FluxOperations.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FluxOperations.Infrastructure.Data.Seeds;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Fluxes.AnyAsync())
            return;

        logger.LogInformation("Seeding database with sample flux operations data...");

        var fluxes = new[]
        {
            Flux.Create("SAP → Datawarehouse ETL", FluxType.ETL,
                "Nightly extraction of SAP transactions to the enterprise DW",
                "SAP ECC", "Azure Synapse", "0 2 * * *"),
            Flux.Create("CRM Real-time Sync", FluxType.Streaming,
                "Real-time customer data sync between Salesforce and the operational DB",
                "Salesforce", "SQL Server", null),
            Flux.Create("Invoice Processing Pipeline", FluxType.DataPipeline,
                "Automated invoice ingestion, validation, and ERP posting",
                "Email / SFTP", "SAP FI", "0 */4 * * *"),
            Flux.Create("Inventory Batch Reconciliation", FluxType.BatchProcessing,
                "Daily reconciliation of warehouse inventory counts",
                "WMS Oracle", "SAP MM", "30 1 * * *"),
            Flux.Create("Payment Gateway Integration", FluxType.ApiIntegration,
                "Stripe webhook processing for payment events",
                "Stripe API", "Internal DB", null),
            Flux.Create("Logistics Partner Feed", FluxType.FileTransfer,
                "SFTP-based shipment status file exchange with 3PL partner",
                "DHL SFTP", "Operations DB", "*/30 * * * *"),
            Flux.Create("Analytics Event Stream", FluxType.MessageQueue,
                "Kafka stream of user analytics events to the data lake",
                "Application Events", "Azure Data Lake", null),
        };

        foreach (var flux in fluxes)
            context.Fluxes.Add(flux);

        await context.SaveChangesAsync();

        fluxes[0].Activate();
        fluxes[0].RecordExecution(12_500, 0.3);

        fluxes[1].Activate();
        fluxes[1].RecordExecution(4_200, 1.1);

        fluxes[2].Activate();
        fluxes[2].RecordExecution(850, 6.5);

        fluxes[3].Activate();
        fluxes[3].RecordExecution(3_000, 0.0);

        fluxes[5].Activate();
        fluxes[5].RecordExecution(95, 2.4);

        fluxes[6].Activate();
        fluxes[6].RecordExecution(88_000, 0.1);

        await context.SaveChangesAsync();

        var alerts = new[]
        {
            Alert.Create(fluxes[2].Id, AlertSeverity.Warning,
                "Error rate exceeded 5% threshold — invoice validation failures detected."),
            Alert.Create(fluxes[2].Id, AlertSeverity.Critical,
                "15 invoices rejected due to missing VAT number in last batch."),
            Alert.Create(fluxes[4].Id, AlertSeverity.Info,
                "Payment Gateway Integration not yet activated — awaiting API key provisioning."),
        };

        foreach (var alert in alerts)
            context.Alerts.Add(alert);

        await context.SaveChangesAsync();

        logger.LogInformation("Database seeding completed. {FluxCount} fluxes, {AlertCount} alerts created.",
            fluxes.Length, alerts.Length);
    }
}
