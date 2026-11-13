import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { applyOtelMiddleware, createTelemetrySdk } from "../src/telemetry.js";

describe("createTelemetrySdk", () => {
  const originalEnv = { ...process.env };

  beforeEach(() => {
    process.env = { ...originalEnv };
  });

  afterEach(() => {
    process.env = originalEnv;
  });

  it("returns null when explicitly disabled", () => {
    const sdk = createTelemetrySdk({ enabled: false });
    expect(sdk).toBeNull();
  });

  it("returns null when disabled via environment variable", () => {
    process.env.SCRIPTBEE__TELEMETRY__ENABLED = "false";
    const sdk = createTelemetrySdk();
    expect(sdk).toBeNull();
  });

  it("returns a NodeSDK instance with default options", () => {
    const sdk = createTelemetrySdk({
      enabled: true,
      serviceName: "test-service",
    });
    expect(sdk).not.toBeNull();
  });

  it("returns a NodeSDK instance when console exporter is enabled", () => {
    const sdk = createTelemetrySdk({
      enabled: true,
      serviceName: "test-service",
      consoleExporterEnabled: true,
    });
    expect(sdk).not.toBeNull();
  });

  it("returns a NodeSDK instance when otlp endpoint is specified", () => {
    const sdk = createTelemetrySdk({
      enabled: true,
      serviceName: "test-service",
      otlpEndpoint: "http://localhost:4318",
    });
    expect(sdk).not.toBeNull();
  });
});

describe("applyOtelMiddleware", () => {
  it("registers middleware on Hono instance without error", () => {
    const app = new Hono();
    expect(() => applyOtelMiddleware(app)).not.toThrow();
  });
});
