# OpenTelemetry Concepts

## Signals

OpenTelemetry defines the following signals:

1. [stable] Traces: span, SpanContext and links between spans.
2. [stable] Metrics: measure (identified by name, description and a unit of values), measurement
   (describes a single value to be collected for a Measure). Counter metric and Gauge metric
   are two types of measures.
3. [stable] Logs
4. [Profiling](https://github.com/open-telemetry/oteps/pull/239): it's a "photo" or "snapshot" of the
   application at a given point in time taken multiple times per second, that "photo" contains the heap
   memory and stack.

> **NOTE:** once an experimental signal has gone through rigorous beta testing, it MAY transition to stable. Long-term
> dependencies MAY now be taken against this signal.

# Important concepts

1. Resource: captures information about the entity for which telemetry is recorded. For
   example, metrics exposed by a Kubernetes container can be linked to a resource that specifies
   the cluster, namespace, pod, and container name.
2. Collector: collector is a set of components that can collect traces, metrics and eventually
   other telemetry data (e.g. logs) from processes instrumented by OpenTelemetry or other
   monitoring/tracing libraries (Jaeger, Prometheus, etc.), do aggregation and smart sampling,
   and export traces and metrics to one or more monitoring/tracing backends. The collector will
   allow to enrich and transform collected telemetry (e.g. add additional attributes or scrub
   personal information).
3. Instrumentation library: The inspiration of the project is to make every library and
   application observable out of the box by having them call OpenTelemetry API directly.
   However, many libraries will not have such integration, and as such there is a need for a
   separate library which would inject such calls, using mechanisms such as wrapping interfaces,
   subscribing to library-specific callbacks, or translating existing telemetry into the
   OpenTelemetry model. A library that enables OpenTelemetry observability for another library is
   called an Instrumentation Library.
4. Baggages: Baggage is a set of key-value pairs that are propagated across process boundaries.
   Baggage is similar to distributed tracing context propagation (for example, traceparent and
   tracestate headers in W3C Trace Context) but it is intended for data that affects the
   operation of the application, rather than the operation of the distributed tracing system.

# OTEL Collector Core VS OTEL Collector Contrib

[OTEL Collector Core Manifest](https://github.com/open-telemetry/opentelemetry-collector-releases/blob/main/distributions/otelcol/manifest.yaml)

- jaegerreceiver, kafkareceiver, otlpreceiver, prometheusreceiver, zipkinreceiver, hostmetrics
- otlpexporter, jaegerexporter, prometheusexporter, zipkinexporter, loggingexporte
- batchprocessor, filterprocessor, memorylimiterprocessor, spanprocessor

[OTEL Collector Contrib Manifest](https://github.com/open-telemetry/opentelemetry-collector-releases/blob/main/distributions/otelcol-contrib/manifest.yaml)

- All from OTEL Collector Core + Vendor specific receivers and exporters

[OTEL Collection Distributions](https://github.com/jpkrohling/otelcol-distributions)

[OTEL Collector Configs](https://github.com/jpkrohling/otelcol-configs/tree/main)

[Ask OTEL - ChatGpt Extension](https://chat.openai.com/g/g-oEq4KWTDe-ask-otel)

# OTEL Pipelines

- Receiver -> Processor -> Exporter
- Receiver: where the data come from. Active or passive. Active receivers go get data in somewhere, or stay listening to
  a file to figure out if there is an update (like a log file). Passive receivers wait for data from the resources.
- Processor: transform the data. Can be a filter, a batcher, a memory limiter, sampler, etc.
- Exporter: where the data goes. Can be a file, a database, a log, etc.

# OTLP Protocol

- OpenTracing didn't define a data format, there was no pattern that could be used to send data from one place to
  another.
- High availability and multi-usage;
- gRPC and HTTP
- gRPC opens a connection and keeps it open to share data.

# Production configuration for OTEL Instrumentation

- Do not configure the destination of the data in the code but in the environment variables;