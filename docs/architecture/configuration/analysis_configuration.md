# Analysis Service Configuration

Most of the variables here are configured automatically by the Gateway Service when a new instance is allocated

## Database

### `ConnectionStrings__mongodb`

- **Type:** `string`
- **Description:** The connection string to MongoDB is configured automatically by the Gateway Service with to reference
  the same Mongodb Server

## User Folder

### `UserFolder__UserFolderPath`

- **Type:** `string`
- **Description:** The absolute path of the host machine to the folder where the ScriptBee data is stored. It is
  automatically configured by the Gateway Service to reference the same volume

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
