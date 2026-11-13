# Analysis Service Configuration

Most of the variables here are configured automatically by the Gateway Service when a new instance is allocated

## Database

### `ConnectionStrings__mongodb`

- **Type:** `string`
- **Description:** The connection string to MongoDB is configured automatically by the Gateway Service with to reference
  the same Mongodb Server

## Instance Configuration

### `SCRIPTBEE__INSTANCEID`

- **Type:** `string`
- **Description:** Automatically configured but the Gateway Service with the id of the instance

### `SCRIPTBEE__PROJECTID`

- **Type:** `string`
- **Description:** Automatically configured but the Gateway Service with the id of project

### `SCRIPTBEE__PROJECTNAME`

- **Type:** `string`
- **Description:** Automatically configured but the Gateway Service with the name of project

## Plugins Configuration

### `SCRIPTBEE__PLUGINS__INSTALLATIONFOLDER`

- **Type:** `string`
- **Default:** `/app/plugins`
- **Description:** The folder inside the analysis instance where plugins are accessed. This folder is typically a read-only view of the central Plugin Cache managed by the Gateway.

## Telemetry

### `SCRIPTBEE__TELEMETRY__ENABLED`

- **Type:** `bool`
- **Default:** `true`
- **Description:** Whether to enable OpenTelemetry tracing and metrics collection. When spawned by the Gateway, this value is automatically forwarded from the Gateway's configuration.

### `SCRIPTBEE__TELEMETRY__OTLPENDPOINT`

- **Type:** `string`
- **Default:** _None_
- **Description:** The OTLP endpoint to export telemetry data to. When spawned by the Gateway, this value is automatically forwarded from the Gateway's configuration.

### `SCRIPTBEE__TELEMETRY__CONSOLEEXPORTERENABLED`

- **Type:** `bool`
- **Default:** `false`
- **Description:** Whether to print traces and metrics to the console output. When spawned by the Gateway, this value is automatically forwarded from the Gateway's configuration.
