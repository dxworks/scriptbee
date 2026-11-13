# SDK Telemetry & OpenTelemetry

ScriptBee Analysis Service SDKs provide out-of-the-box OpenTelemetry tracing and metric collection for C#, Python, and TypeScript. When an Analysis instance is launched by the Gateway, telemetry configuration (such as OTLP collector endpoints) is automatically propagated via environment variables.

---

## Configuration & Environment Variables

All SDKs read the standard ScriptBee telemetry environment variables:

| Environment Variable                           | Type     | Default              | Description                                                                                               |
| ---------------------------------------------- | -------- | -------------------- | --------------------------------------------------------------------------------------------------------- |
| `SCRIPTBEE__TELEMETRY__ENABLED`                | `bool`   | `true`               | Set to `false` to opt out and disable all telemetry.                                                      |
| `SCRIPTBEE__TELEMETRY__OTLPENDPOINT`           | `string` | _None_               | The OpenTelemetry Collector OTLP endpoint (e.g. `http://otel-collector:4318` or `http://localhost:4317`). |
| `SCRIPTBEE__TELEMETRY__CONSOLEEXPORTERENABLED` | `bool`   | `false`              | When `true`, prints traces and metrics to stdout for local debugging.                                     |
| `SCRIPTBEE__TELEMETRY__SERVICE_NAME`           | `string` | `scriptbee.analysis` | The OpenTelemetry service name.                                                                           |

---

## C# Analysis Service SDK

### Setup

OpenTelemetry support is built into `DxWorks.ScriptBee.Analysis.Sdk`. Call `AddAnalysisSdkTelemetry()` on your `IServiceCollection`:

```csharp
using DxWorks.ScriptBee.Analysis.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAnalysisSdk(builder.Configuration)
    .AddAnalysisSdkTelemetry(builder.Configuration, "my-analysis-service");
```

### Opting Out

In your `appsettings.json`:

```json
{
  "ScriptBee": {
    "Telemetry": {
      "Enabled": false
    }
  }
}
```

Or via environment variable:

```dotenv
SCRIPTBEE__TELEMETRY__ENABLED=false
```

---

## Python Analysis Service SDK

### Installation

Install the SDK with the optional `telemetry` dependency group:

```bash
pip install "scriptbee-analysis-sdk[telemetry]"
```

### Setup

Call `setup_telemetry()` during your application startup:

```python
from fastapi import FastAPI
from scriptbee_analysis_sdk import create_analysis_sdk_router, setup_telemetry

app = FastAPI()

# Instrument traces and metrics (HTTP requests, HTTPX client calls, and runtime metrics)
setup_telemetry(app=app)

# Mount SDK router
app.include_router(create_analysis_sdk_router(container))
```

### Explicit Collector Configuration

```python
setup_telemetry(
    app=app,
    service_name="custom-python-analysis",
    otlp_endpoint="http://otel-collector:4318",
    console_exporter_enabled=False,
)
```

### Opting Out

```python
setup_telemetry(enabled=False)
```

Or via environment variable:

```dotenv
SCRIPTBEE__TELEMETRY__ENABLED=false
```

---

## TypeScript Analysis Service SDK

### Installation

Install OpenTelemetry and `@hono/otel` alongside the SDK:

```bash
npm install @hono/otel @opentelemetry/sdk-node @opentelemetry/resources @opentelemetry/semantic-conventions @opentelemetry/exporter-trace-otlp-http @opentelemetry/exporter-metrics-otlp-http @opentelemetry/auto-instrumentations-node
```

### Setup

Bootstrap the OpenTelemetry SDK before initializing your server, and attach the Hono middleware:

```typescript
// instrumentation.ts
import { createTelemetrySdk } from '@dxworks/scriptbee-analysis-sdk';

const sdk = createTelemetrySdk({
  serviceName: 'custom-ts-analysis',
});
sdk?.start();
```

```typescript
// index.ts
import { Hono } from 'hono';
import { applyOtelMiddleware, createAnalysisSdkRouter } from '@dxworks/scriptbee-analysis-sdk';

const app = new Hono();

// Apply OpenTelemetry route-level HTTP tracing middleware
applyOtelMiddleware(app);

// Mount SDK router
app.route('/', createAnalysisSdkRouter(container));

export default app;
```

### Opting Out

```typescript
const sdk = createTelemetrySdk({ enabled: false });
// Returns null, no OpenTelemetry providers are registered
```

Or via environment variable:

```dotenv
SCRIPTBEE__TELEMETRY__ENABLED=false
```
