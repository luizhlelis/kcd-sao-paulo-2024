using DotNetCore.CAP.Internal;
using Microsoft.EntityFrameworkCore;
using Catalog.Consumer;
using Catalog.Consumer.Consumers;
using Catalog.Consumer.Domain.Events;
using Catalog.Consumer.Domain.Services;
using Catalog.Consumer.Idempotency;
using Catalog.Consumer.Infrastructure;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables()
    .Build();

var host = Host.CreateDefaultBuilder(args)
        .ConfigureLogging((hostingContext, logging) =>
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
        })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        // Database
        services.AddDbContext<CatalogContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CatalogContext")));

        // Business rules
        services
            .AddScoped<OrderCreatedConsumer>()
            .AddConsumerService<OrderCreated, OrderCreatedService>(
                options =>
                {
                    options.UseIdempotency<CatalogContext>();
                });

        // CAP
        services.AddCap(options =>
            {
                options.UseEntityFramework<CatalogContext>();

                options.DefaultGroupName = "catalog";

                options.UseRabbitMQ(o =>
                {
                    o.HostName = configuration.GetValue<string>("RabbitMQ:HostName") ?? string.Empty;
                    o.UserName = configuration.GetValue<string>("RabbitMq:UserName") ?? string.Empty;
                    o.Password = configuration.GetValue<string>("RabbitMq:Password") ?? string.Empty;
                    o.Port = configuration.GetValue<int>("RabbitMQ:Port");
                    o.ExchangeName = configuration.GetValue<string>("RabbitMQ:ExchangeName") ?? string.Empty;
                    o.VirtualHost = configuration.GetValue<string>("RabbitMq:VHost") ?? string.Empty;

                    o.CustomHeaders = e => new List<KeyValuePair<string, string>>
                    {
                        new(DotNetCore.CAP.Messages.Headers.MessageId, new SnowflakeId().NextId().ToString()),
                        new(DotNetCore.CAP.Messages.Headers.MessageName, e.RoutingKey)
                    };
                });
            })
            .AddSubscribeFilter<BootstrapFilter>();

        // OpenTelemetry
        OpenTelemetry.Sdk.SetDefaultTextMapPropagator(new TraceContextPropagator());
        var otlpEndpoint = new Uri(configuration.GetValue<string>("Otlp:Endpoint") ?? string.Empty);
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(configuration.GetValue<string>("Otlp:ServiceName") ?? string.Empty);
        services.AddSingleton(
            TracerProvider.Default.GetTracer(configuration.GetValue<string>("Otlp:ServiceName")));
        services
            .AddOpenTelemetry()
            .WithMetrics(m =>
                m.AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetResourceBuilder(resourceBuilder)
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = otlpEndpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
            ).WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation()
                    .AddCapInstrumentation()
                    .SetResourceBuilder(resourceBuilder)
                    .AddHttpClientInstrumentation(options => options.RecordException = true)
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.SetDbStatementForText = true;
                    })
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = otlpEndpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
    })
    .Build();

await host.RunAsync();
