# Gateway Service Configuration

## Database

### `ConnectionStrings__mongodb`

- **Type:** `string`
- **Default:** `mongodb://root:example@localhost:27017/ScriptBee?authSource=admin`
- **Description:** The connection string to MongoDB. Must be configured to match the Mongodb server configurations

## Analysis Configuration

### `SCRIPTBEE__ANALYSIS__DRIVER`

- **Type:** `string`
- **Default:** `Docker`
- **Description:** The driver used to manage analysis instances. Currently only `Docker` is supported.

### `SCRIPTBEE__ANALYSIS__IMAGE`

- **Type:** `string`
- **Default:** `dxworks/scriptbee-analysis:latest`
- **Description:** The Docker image used for analysis instances.

### `SCRIPTBEE__ANALYSIS__DOCKER__DOCKERSOCKET`

- **Type:** `string`
- **Default:** `unix:///var/run/docker.sock`
- **Description:** URI to the Docker daemon socket.

### `SCRIPTBEE__ANALYSIS__DOCKER__PORT`

- **Type:** `int`
- **Default:** `80`
- **Description:** The internal port the analysis container listens on.

### `SCRIPTBEE__ANALYSIS__DOCKER__NETWORK`

- **Type:** `string`
- **Default:** _None_
- **Description:** The Docker network name to attach containers to (required if using a custom network for MongoDB).

### `SCRIPTBEE__ANALYSIS__DOCKER__MONGODBCONNECTIONSTRING`

- **Type:** `string`
- **Default:** _Inherits Gateway's if omitted_
- **Description:** MongoDB connection string for the analysis instance.

### `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERVOLUMEPATH`

- **Type:** `string`
- **Default:** _None_
- **Description:** The mount point for project data **inside** the analysis container.

### `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH`

- **Type:** `string`
- **Default:** _None_
- **Description:** The absolute path on the **host machine** where project data is stored.

> [!IMPORTANT]
> The `SCRIPTBEE__ANALYSIS__DOCKER__USERFOLDERHOSTPATH` must be an **absolute path** on your host machine. This is
> because the Gateway tells the Docker daemon to mount this path into the new analysis containers. If this is incorrect,
> the analysis service will not be able to find your scripts or models.

### `SCRIPTBEE__ANALYSIS__DOCKER__PLUGINSVOLUME`

- **Type:** `string`
- **Default:** `scriptbee-plugins`
- **Description:** The name of the volume that will be created and shared for installing plugins. This will be mounted
  in Analysis Service as a readonly path to access the plugins

### `SCRIPTBEE__ANALYSIS__DOCKER__HOSTCONFIG`

> [!NOTE]
> Note that you can configure docker options such as resource limits and environment variables for the analysis
> containers by using the `SCRIPTBEE__ANALYSIS__DOCKER__HOSTCONFIG` configuration. This allows for advanced users to
> have
> full control over the Docker container configuration without the need for the Gateway to explicitly support each
> option.
> Please
> see [Docker Host Config](https://github.com/dotnet/Docker.DotNet/blob/master/src/Docker.DotNet/Models/HostConfig.Generated.cs)
> for all available options that can be configured through this setting.

For example, you can se the memory limit like this

```
SCRIPTBEE__ANALYSIS__DOCKER__HOSTCONFIG__MEMORY=536870912
```

## Instance Configuration

### `SCRIPTBEE__INSTANCE__POLLINGDELAYMILLISECONDS`

- **Type:** `long`
- **Default:** `1000`
- **Description:** The polling interval to check when the analysis instance is ready so that the plugins can be
  automatically installed there

### `SCRIPTBEE__INSTANCE__ANALYSISSTATUSMONITORINTERVALMILLISECONDS`

- **Type:** `int`
- **Default:** `2000`
- **Description:** The interval in milliseconds at which the Gateway polls MongoDB for running analyses to push updates via
  SignalR.

## Plugins Configuration

### `SCRIPTBEE__PLUGINS__INSTALLATIONFOLDER`

- **Type:** `string`
- **Default:** `[User Profile Folder]/.scriptbee/plugins`
- **Description:** The folder where all downloaded plugins are stored (the "Cache"). This folder is shared between the
  Gateway and all Analysis instances.

### `SCRIPTBEE__PLUGINS__GATEWAYINSTALLATIONFOLDER`

- **Type:** `string`
- **Default:** `[User Profile Folder]/.scriptbee/gateway/plugins`
- **Description:** The folder for plugins that are actively "Enabled" in the Gateway. When you enable a plugin from the
  UI, it is copied here so the Gateway can load it.
