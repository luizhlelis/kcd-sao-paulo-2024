using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Order.Api.Infrastructure;

namespace Order.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Swagger
        var openApiInfo = new OpenApiInfo();
        Configuration.Bind("OpenApiInfo", openApiInfo);
        services.AddSingleton(openApiInfo);

        // Databases
        services.AddDbContext<OrderContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("OrderContext")));

        // CAP
        services.AddCap(x =>
        {
            x.UseEntityFramework<OrderContext>();

            x.UseRabbitMQ(o =>
            {
                o.HostName = Configuration.GetValue<string>("RabbitMq:HostName") ?? string.Empty;
                o.UserName = Configuration.GetValue<string>("RabbitMq:UserName") ?? string.Empty;
                o.Password = Configuration.GetValue<string>("RabbitMq:Password") ?? string.Empty;
                o.Port = Configuration.GetValue<int>("RabbitMq:Port");
                o.ExchangeName = Configuration.GetValue<string>("RabbitMq:ExchangeName") ??
                                 string.Empty;
                o.VirtualHost = Configuration.GetValue<string>("RabbitMq:VHost") ?? string.Empty;
            });
        });

        // OpenTelemetry
        OpenTelemetry.Sdk.SetDefaultTextMapPropagator(new TraceContextPropagator());
        var otlpEndpoint = new Uri(Configuration.GetValue<string>("Otlp:Endpoint") ?? string.Empty);
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(Configuration.GetValue<string>("Otlp:ServiceName") ?? string.Empty);
        services.AddSingleton(
            TracerProvider.Default.GetTracer(Configuration.GetValue<string>("Otlp:ServiceName")));
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

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddControllers();
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", openApiInfo); });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
            c.SwaggerEndpoint(
                $"/swagger/v1/swagger.json",
                "Order.Api v1")
        );

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
