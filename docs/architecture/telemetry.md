# OpenTelemetry & Telemetry

ScriptBee uses [OpenTelemetry (OTel)](https://opentelemetry.io/) to provide observability across its services, including distributed tracing and metric collection.

## Overview

Observability in ScriptBee is structured around OpenTelemetry standards:

- **Distributed Tracing**: Automatically instruments incoming HTTP requests and outgoing HTTP calls across both Gateway and Analysis services. Distributed context propagation allows traces to link Gateway requests with corresponding actions in spawned Analysis services.
- **Metric Collection**: Gathers ASP.NET Core request metrics, HTTP client metrics, and .NET runtime metrics (such as GC, thread pool, memory allocations).
- **Vendor Agnostic**: Exporter uses standard OTLP (OpenTelemetry Protocol), allowing integration with collectors, Jaeger, Prometheus, Grafana Tempo, SigNoz, Datadog, or any OTLP-compatible backend.

## Configuration

Telemetry can be configured through `appsettings.json` or environment variables:

| Configuration Key                            | Environment Variable                           | Type     | Default | Description                                                                                                                   |
| -------------------------------------------- | ---------------------------------------------- | -------- | ------- | ----------------------------------------------------------------------------------------------------------------------------- |
| `ScriptBee:Telemetry:Enabled`                | `SCRIPTBEE__TELEMETRY__ENABLED`                | `bool`   | `true`  | Enables or disables tracing and metrics collection.                                                                           |
| `ScriptBee:Telemetry:OtlpEndpoint`           | `SCRIPTBEE__TELEMETRY__OTLPENDPOINT`           | `string` | _None_  | The endpoint URL for the OTLP exporter (e.g. `http://otel-collector:4318` for HTTP or `http://otel-collector:4317` for gRPC). |
| `ScriptBee:Telemetry:ConsoleExporterEnabled` | `SCRIPTBEE__TELEMETRY__CONSOLEEXPORTERENABLED` | `bool`   | `false` | Enables printing traces and metrics to stdout for debugging.                                                                  |

### Opting Out

If you wish to opt out of telemetry entirely, set `SCRIPTBEE__TELEMETRY__ENABLED` to `false`:

```dotenv
SCRIPTBEE__TELEMETRY__ENABLED=false
```

When disabled, no OpenTelemetry tracer providers, meter providers, or background exporters are registered.

### Propagation to Analysis Instances

When the Gateway allocates and launches an Analysis instance (whether using Docker or Kubernetes), it automatically forwards the configured telemetry environment variables:

- `ScriptBee__Telemetry__Enabled`
- `ScriptBee__Telemetry__OtlpEndpoint` (if configured)
- `ScriptBee__Telemetry__ConsoleExporterEnabled` (if configured)

This ensures that both Gateway and Analysis instances report to the same OpenTelemetry collector without requiring manual per-instance configuration.

## Setting Up an OpenTelemetry Collector

To collect and visualize traces and metrics locally, you can deploy an OpenTelemetry Collector and Jaeger alongside ScriptBee.

### Example Docker Compose

```yaml
services:
  otel-collector:
    image: otel/opentelemetry-collector-contrib:latest
    container_name: scriptbee-otel-collector
    restart: unless-stopped
    command: ['--config=/etc/otelcol-contrib/config.yaml']
    volumes:
      - ./otel-collector-config.yaml:/etc/otelcol-contrib/config.yaml
    ports:
      - '4317:4317' # OTLP gRPC receiver
      - '4318:4318' # OTLP HTTP receiver

  jaeger:
    image: jaegertracing/all-in-one:latest
    container_name: scriptbee-jaeger
    restart: unless-stopped
    ports:
      - '16686:16686' # Jaeger Web UI
      - '14250:14250' # Jaeger gRPC model
```

### Example `otel-collector-config.yaml`

```yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317
      http:
        endpoint: 0.0.0.0:4318

processors:
  batch:

exporters:
  otlp:
    endpoint: jaeger:4317
    tls:
      insecure: true
  debug:
    verbosity: detailed

service:
  pipelines:
    traces:
      receivers: [otlp]
      processors: [batch]
      exporters: [otlp, debug]
    metrics:
      receivers: [otlp]
      processors: [batch]
      exporters: [debug]
```

### Pointing ScriptBee to the Collector

Configure the Gateway environment variable:

```dotenv
SCRIPTBEE__TELEMETRY__ENABLED=true
SCRIPTBEE__TELEMETRY__OTLPENDPOINT=http://otel-collector:4318
```
