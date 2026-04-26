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
