using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Order.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddOpenTelemetry(options =>
                    {
                        var resourceBuilder = ResourceBuilder.CreateDefault()
                            .AddService(
                                hostingContext.Configuration.GetValue<string>("Otlp:ServiceName") ??
                                string.Empty);
                        options.IncludeScopes = true;
                        options.ParseStateValues = true;
                        options.IncludeFormattedMessage = true;
                        options.SetResourceBuilder(resourceBuilder);
                        options.AddConsoleExporter();
                        options.AddOtlpExporter(otlpExporterOptions =>
                        {
                            otlpExporterOptions.Endpoint =
                                new Uri(hostingContext.Configuration.GetValue<string>(
                                    "Otlp:Endpoint") ?? string.Empty);
                            otlpExporterOptions.Protocol = OtlpExportProtocol.Grpc;
                        });
                    });
                });
            });
    }
}
