# RBAC Configuration

> [!IMPORTANT]
> Role-Based Access Control (RBAC) and Authentication features are handled by a **separate, external service**. The
> implementation details here represent how ScriptBee interacts with that service.

ScriptBee uses OPA (Open Policy Agent) for RBAC and authentication, determining their roles and permissions.

See [Authentication](https://dxworks.org/scriptbee/architecture/configuration/gateway_configuration.html#authentication)
for more information on how to configure the authentication and authorization service.

See [Architecture Diagram](https://dxworks.org/scriptbee/architecture/diagram.html) to understand the interaction
between ScriptBee and the authentication and authorization service.

An example of the default rules can be found in
the [OPA rules](https://github.com/dxworks/scriptbee/blob/master/quickstart/full/policy.rego) file.

The following sections describe the roles and permissions that are used in ScriptBee. The roles can be defined in the
OPA server and assigned to users. The permissions are used to determine what actions a user can perform in ScriptBee.

## Authorization Contracts

ScriptBee uses the following authorization contracts to determine what actions a user can perform.

### External Authorization Url

See [ExternalAuthorizationUrl Config](./configuration/gateway_configuration.md#authentication__externalauthorizationurl)
for more details

#### Request

POST request with body example:

```json
{
  "input": {
    "subject": {
      "user_id": "user-123",
      "groups": ["admins"]
    },
    "action": "project:view",
    "resource": {
      "type": "project",
      "id": "project-123",
      "role": "Administrator"
    }
  }
}
```

Or

```json
{
  "input": {
    "subject": {
      "user_id": "user-123",
      "groups": ["admins"]
    },
    "action": "gateway_plugins:view",
    "resource": {
      "type": "global"
    }
  }
}
```

The possible values for the `action` field are defined in the [Permissions](#permissions) section.

The possible values for the `resource.type` are:

- global
- project

#### Response

The response is expected to be in the format:

```json
{
  "result": true
}
```

### Permissions Url

See [PermissionsUrl Config](./configuration/gateway_configuration.md#authentication__permissionsurl)
for more details

#### Request

POST request with body example:

```json
{
  "input": {
    "subject": {
      "user_id": "user-123",
      "groups": []
    },
    "resource": {
      "type": "project",
      "id": "project-123",
      "role": "Editor"
    }
  }
}
```

#### Response

The response is expected to be in the format:

```json
{
  "result": ["project:view", "project:edit", "project:delete"]
}
```

### Roles Url

See [RolesUrl Config](./configuration/gateway_configuration.md#authentication__rolesurl) for more details

#### Request

A GET request

#### Response

The response is expected to be in the format:

```json
{
  "result": [
    {
      "id": "Editor",
      "description": "User that can manage projects they belong to."
    },
    {
      "id": "Analyst",
      "description": "User that can perform analysis tasks on allowed projects."
    },
    {
      "id": "Viewer",
      "description": "User that can view only allowed projects and their analysis."
    }
  ]
}
```

### Default Creator Role Url

#### Request

See [DefaultCreatorRoleUrl Config](./configuration/gateway_configuration.md#authentication__defaultcreatorroleurl)
for more details

A GET request

#### Response

The response is expected to be in the format:

```json
{
  "result": "Editor"
}
```

## Roles

Detailed here are the possible roles a user can have in ScriptBee.

- Admin - user that can perform any action
- Editor - user that can perform project management on project they belong to
- Analyst - user that can perform different analysis tasks on allowed projects
- Viewer - user that can view the results of the analysis

> Note: The roles are only as examples, and can be defined in the OPA server. The roles can be assigned to users in the
> OPA server. The permissions are the ones that are actually used in ScriptBee to determine what actions a user can
> perform. The roles can be defined in the OPA server and assigned to users. The permissions are used to determine what
> actions a user can perform in ScriptBee.

## Permissions

### Project

| Permission     | Admin   | Editor  | Analyst | Viewer  |
| -------------- | ------- | ------- | ------- | ------- |
| project:create | &check; | &check; | &check; | &check; |
| project:view   | &check; | &check; | &check; | &check; |
| project:edit   | &check; | &check; |         |         |
| project:delete | &check; | &check; |         |         |

### Scripts

| Permission    | Admin   | Editor  | Analyst | Viewer  |
| ------------- | ------- | ------- | ------- | ------- |
| script:view   | &check; | &check; | &check; | &check; |
| script:create | &check; | &check; |         |         |
| script:edit   | &check; | &check; |         |         |
| script:delete | &check; | &check; |         |         |

### Model

| Permission   | Admin   | Editor  | Analyst | Viewer |
| ------------ | ------- | ------- | ------- | ------ |
| model:view   | &check; | &check; | &check; |        |
| model:upload | &check; | &check; | &check; |        |
| model:load   | &check; | &check; | &check; |        |
| model:link   | &check; | &check; | &check; |        |
| model:clear  | &check; | &check; | &check; |        |

### Analysis

| Permission      | Admin   | Editor  | Analyst | Viewer  |
| --------------- | ------- | ------- | ------- | ------- |
| analysis:view   | &check; | &check; | &check; | &check; |
| analysis:run    | &check; | &check; | &check; |         |
| analysis:delete | &check; | &check; |         |         |

### Plugins

| Permission               | Admin   | Editor  | Analyst | Viewer |
| ------------------------ | ------- | ------- | ------- | ------ |
| plugin:view              | &check; | &check; | &check; |        |
| plugin:install           | &check; | &check; |         |        |
| plugin:uninstall         | &check; | &check; |         |        |
| plugin:configure         | &check; | &check; |         |        |
| gateway_plugin:view      | &check; |         |         |        |
| gateway_plugin:install   | &check; |         |         |        |
| gateway_plugin:uninstall | &check; |         |         |        |
| gateway_plugin:configure | &check; |         |         |        |

### Token management

| Permission   | Admin   | Editor  | Analyst | Viewer |
| ------------ | ------- | ------- | ------- | ------ |
| token:create | &check; | &check; |         |        |
| token:delete | &check; | &check; |         |        |
