import os
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from fastapi import FastAPI


def setup_telemetry(
    service_name: str | None = None,
    otlp_endpoint: str | None = None,
    console_exporter_enabled: bool | None = None,
    enabled: bool | None = None,
    app: "FastAPI | None" = None,
) -> bool:
    is_telemetry_enabled = (
        enabled
        if enabled is not None
        else os.getenv("SCRIPTBEE__TELEMETRY__ENABLED", "true").lower() in ("true", "1", "yes")
    )

    if not is_telemetry_enabled:
        return False

    from opentelemetry import metrics, trace
    from opentelemetry.exporter.otlp.proto.grpc.metric_exporter import (
        OTLPMetricExporter as GrpcMetricExporter,
    )
    from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import (
        OTLPSpanExporter as GrpcSpanExporter,
    )
    from opentelemetry.exporter.otlp.proto.http.metric_exporter import (
        OTLPMetricExporter as HttpMetricExporter,
    )
    from opentelemetry.exporter.otlp.proto.http.trace_exporter import (
        OTLPSpanExporter as HttpSpanExporter,
    )
    from opentelemetry.instrumentation.fastapi import FastAPIInstrumentor
    from opentelemetry.instrumentation.httpx import HTTPXClientInstrumentor
    from opentelemetry.sdk.metrics import MeterProvider
    from opentelemetry.sdk.metrics.export import (
        ConsoleMetricExporter,
        PeriodicExportingMetricReader,
    )
    from opentelemetry.sdk.resources import SERVICE_NAME, Resource
    from opentelemetry.sdk.trace import TracerProvider
    from opentelemetry.sdk.trace.export import BatchSpanProcessor, ConsoleSpanExporter

    resolved_service_name = (
        service_name
        or os.getenv("SCRIPTBEE__TELEMETRY__SERVICE_NAME")
        or os.getenv("OTEL_SERVICE_NAME")
        or "scriptbee.analysis"
    )
    resolved_otlp_endpoint = (
        otlp_endpoint
        or os.getenv("SCRIPTBEE__TELEMETRY__OTLPENDPOINT")
        or os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
    )
    resolved_console_exporter = (
        console_exporter_enabled
        if console_exporter_enabled is not None
        else os.getenv("SCRIPTBEE__TELEMETRY__CONSOLEEXPORTERENABLED", "false").lower()
        in ("true", "1", "yes")
    )

    resource = Resource(attributes={SERVICE_NAME: resolved_service_name})
    tracer_provider = TracerProvider(resource=resource)
    metric_readers = []

    if resolved_otlp_endpoint:
        if resolved_otlp_endpoint.startswith("http://") or resolved_otlp_endpoint.startswith(
            "https://"
        ):
            trace_url = (
                resolved_otlp_endpoint
                if resolved_otlp_endpoint.endswith("/v1/traces")
                else f"{resolved_otlp_endpoint.rstrip('/')}/v1/traces"
            )
            metric_url = (
                resolved_otlp_endpoint
                if resolved_otlp_endpoint.endswith("/v1/metrics")
                else f"{resolved_otlp_endpoint.rstrip('/')}/v1/metrics"
            )
            tracer_provider.add_span_processor(
                BatchSpanProcessor(HttpSpanExporter(endpoint=trace_url))
            )
            metric_readers.append(
                PeriodicExportingMetricReader(HttpMetricExporter(endpoint=metric_url))
            )
        else:
            clean_grpc_endpoint = resolved_otlp_endpoint.replace("grpc://", "")
            tracer_provider.add_span_processor(
                BatchSpanProcessor(GrpcSpanExporter(endpoint=clean_grpc_endpoint, insecure=True))
            )
            metric_readers.append(
                PeriodicExportingMetricReader(
                    GrpcMetricExporter(endpoint=clean_grpc_endpoint, insecure=True)
                )
            )

    if resolved_console_exporter:
        tracer_provider.add_span_processor(BatchSpanProcessor(ConsoleSpanExporter()))
        metric_readers.append(PeriodicExportingMetricReader(ConsoleMetricExporter()))

    meter_provider = MeterProvider(resource=resource, metric_readers=metric_readers)

    trace.set_tracer_provider(tracer_provider)
    metrics.set_meter_provider(meter_provider)

    HTTPXClientInstrumentor().instrument()

    if app is not None:
        FastAPIInstrumentor.instrument_app(app)
    else:
        FastAPIInstrumentor().instrument()

    return True
