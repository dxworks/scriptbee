import { NodeSDK } from "@opentelemetry/sdk-node";
import { resourceFromAttributes } from "@opentelemetry/resources";
import { ATTR_SERVICE_NAME } from "@opentelemetry/semantic-conventions";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-http";
import { OTLPMetricExporter } from "@opentelemetry/exporter-metrics-otlp-http";
import {
  ConsoleMetricExporter,
  PeriodicExportingMetricReader,
} from "@opentelemetry/sdk-metrics";
import { ConsoleSpanExporter } from "@opentelemetry/sdk-trace";
import { getNodeAutoInstrumentations } from "@opentelemetry/auto-instrumentations-node";
import { httpInstrumentationMiddleware } from "@hono/otel";
import type { Hono } from "hono";

export interface TelemetryOptions {
  readonly enabled?: boolean;
  readonly serviceName?: string;
  readonly otlpEndpoint?: string;
  readonly consoleExporterEnabled?: boolean;
}

export function createTelemetrySdk(
  options: TelemetryOptions = {},
): NodeSDK | null {
  const isEnabled =
    options.enabled ??
    (process.env.SCRIPTBEE__TELEMETRY__ENABLED
      ? ["true", "1", "yes"].includes(
          process.env.SCRIPTBEE__TELEMETRY__ENABLED.toLowerCase(),
        )
      : true);

  if (!isEnabled) {
    return null;
  }

  const serviceName =
    options.serviceName ??
    process.env.SCRIPTBEE__TELEMETRY__SERVICE_NAME ??
    process.env.OTEL_SERVICE_NAME ??
    "scriptbee.analysis";

  const otlpEndpoint =
    options.otlpEndpoint ??
    process.env.SCRIPTBEE__TELEMETRY__OTLPENDPOINT ??
    process.env.OTEL_EXPORTER_OTLP_ENDPOINT;

  const isConsoleExporterEnabled =
    options.consoleExporterEnabled ??
    (process.env.SCRIPTBEE__TELEMETRY__CONSOLEEXPORTERENABLED
      ? ["true", "1", "yes"].includes(
          process.env.SCRIPTBEE__TELEMETRY__CONSOLEEXPORTERENABLED.toLowerCase(),
        )
      : false);

  const resource = resourceFromAttributes({
    [ATTR_SERVICE_NAME]: serviceName,
  });

  const traceExporter = otlpEndpoint
    ? new OTLPTraceExporter({
        url: otlpEndpoint.endsWith("/v1/traces")
          ? otlpEndpoint
          : `${otlpEndpoint.replace(/\/+$/, "")}/v1/traces`,
      })
    : isConsoleExporterEnabled
      ? new ConsoleSpanExporter()
      : undefined;

  const metricReader = otlpEndpoint
    ? new PeriodicExportingMetricReader({
        exporter: new OTLPMetricExporter({
          url: otlpEndpoint.endsWith("/v1/metrics")
            ? otlpEndpoint
            : `${otlpEndpoint.replace(/\/+$/, "")}/v1/metrics`,
        }),
      })
    : isConsoleExporterEnabled
      ? new PeriodicExportingMetricReader({
          exporter: new ConsoleMetricExporter(),
        })
      : undefined;

  return new NodeSDK({
    resource,
    ...(traceExporter ? { traceExporter } : {}),
    ...(metricReader ? { metricReader } : {}),
    instrumentations: [
      getNodeAutoInstrumentations({
        "@opentelemetry/instrumentation-http": {
          disableIncomingRequestInstrumentation: true,
        },
      }),
    ],
  });
}

export function applyOtelMiddleware(app: Hono): void {
  app.use("*", httpInstrumentationMiddleware());
}
