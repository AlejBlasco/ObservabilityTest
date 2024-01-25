using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        var otel = builder.Services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        var apiMetricsMeterName = builder.Configuration["Metrics:ApiMetrics:Meter:Name"];

        otel.ConfigureResource(resource => resource
            .AddService(serviceName: builder.Environment.ApplicationName));

        // Add Azure Monitor
        otel.UseAzureMonitor();

        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics => metrics
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            // Custom metrics
            .AddMeter(apiMetricsMeterName!)
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            // Set Prometheus
            .AddPrometheusExporter());

        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        var apiMetricsActivitySourceName = builder.Configuration["Metrics:ApiMetrics:ActivitySource:Name"];
        var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];

        otel.WithTracing(tracing =>
        {
            // Default traces
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            // Custom traces
            tracing.AddSource(apiMetricsActivitySourceName!);
            // Set Endpoints
            if (tracingOtlpEndpoint != null)
            {
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                });
            }
            // Add Console Endpoint
            tracing.AddConsoleExporter();
        });

        return services;
    }
}