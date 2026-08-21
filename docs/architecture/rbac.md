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

Request body example:

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
    "action": "plugins:view",
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

The response is expected to be in the format:

```json
{
  "allow": true
}
```

## Roles

Detailed here are the possible roles a user can have in ScriptBee.

- Administrator - user that can perform any action
- Manager - user that can perform project management on project they belong to
- Analyst - user that can perform different analysis tasks on allowed projects
- Auditor - user that can view only allowed projects

> Note: The roles are only as examples, and can be defined in the OPA server. The roles can be assigned to users in the
> OPA server. The permissions are the ones that are actually used in ScriptBee to determine what actions a user can
> perform. The roles can be defined in the OPA server and assigned to users. The permissions are used to determine what
> actions a user can perform in ScriptBee.

## Permissions

### Project

| Permission              | Admin   | Manager | Analyst | Auditor |
| ----------------------- | ------- | ------- | ------- | ------- |
| project:view            | &check; | &check; | &check; | &check; |
| project:edit            | &check; | &check; |         |         |
| project:remove          | &check; | &check; |         |         |
| project:load_model      | &check; | &check; | &check; |         |
| project:link_model      | &check; | &check; | &check; |         |
| project:generate_script | &check; | &check; | &check; |         |
| project:create_script   | &check; | &check; | &check; |         |
| project:edit_script     | &check; | &check; | &check; |         |
| project:delete_script   | &check; | &check; | &check; |         |

### Analysis

| Permission      | Admin   | Manager | Analyst | Auditor |
| --------------- | ------- | ------- | ------- | ------- |
| analysis:view   | &check; | &check; | &check; | &check; |
| analysis:run    |         | &check; | &check; | &check; |
| analysis:remove |         | &check; | &check; | &check; |

### Token management

| Permission            | Admin | Manager | Analyst | Auditor |
| --------------------- | ----- | ------- | ------- | ------- |
| analysis_token:create |       |         | &check; | &check; |
| analysis_token:delete |       |         | &check; | &check; |
