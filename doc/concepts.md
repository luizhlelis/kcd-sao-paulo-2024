# OpenTelemetry Concepts

## Signals

OpenTelemetry defines the following signals:

1. Traces: span, SpanContext and links between spans.
2. Metrics: measure (identified by name, description and a unit of values), measurement
(describes a single value to be collected for a Measure). Counter metric and Gauge metric
are two types of measures.
3. Logs
4. Baggage

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

