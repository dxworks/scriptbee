from fastapi import FastAPI

from scriptbee_analysis_sdk.telemetry import setup_telemetry


def test_setup_telemetry_disabled_explicit():
    result = setup_telemetry(enabled=False)
    assert result is False


def test_setup_telemetry_disabled_env(monkeypatch):
    monkeypatch.setenv("SCRIPTBEE__TELEMETRY__ENABLED", "false")
    result = setup_telemetry()
    assert result is False


def test_setup_telemetry_enabled_default():
    result = setup_telemetry(enabled=True, service_name="test-service")
    assert result is True


def test_setup_telemetry_with_console_exporter():
    result = setup_telemetry(
        enabled=True,
        service_name="test-service",
        console_exporter_enabled=True,
    )
    assert result is True


def test_setup_telemetry_with_http_otlp_endpoint():
    result = setup_telemetry(
        enabled=True,
        service_name="test-service",
        otlp_endpoint="http://localhost:4318",
    )
    assert result is True


def test_setup_telemetry_with_grpc_otlp_endpoint():
    result = setup_telemetry(
        enabled=True,
        service_name="test-service",
        otlp_endpoint="localhost:4317",
    )
    assert result is True


def test_setup_telemetry_with_fastapi_app():
    app = FastAPI()
    result = setup_telemetry(
        enabled=True,
        service_name="test-service",
        app=app,
    )
    assert result is True
