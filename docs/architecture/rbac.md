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

## Roles

Detailed here are the possible roles a user can have in ScriptBee.

- Administrator - user that can perform any action
- Manager - user that can perform project management on project they belong to
- Analyst - user that can perform different analysis tasks on allowed projects
- Auditor - user that can view only allowed projects

## Permissions

### Project

| Permission      | Admin   | Manager | Analyst | Auditor |
| --------------- | ------- | ------- | ------- | ------- |
| view_project    | &check; | &check; | &check; | &check; |
| edit_project    | &check; | &check; |         |         |
| remove_project  | &check; | &check; |         |         |
| load_model      | &check; | &check; | &check; |         |
| link_model      | &check; | &check; | &check; |         |
| generate_script | &check; | &check; | &check; |         |
| create_script   | &check; | &check; | &check; |         |
| edit_script     | &check; | &check; | &check; |         |
| delete_script   | &check; | &check; | &check; |         |

### Analysis

| Permission      | Admin   | Manager | Analyst | Auditor |
| --------------- | ------- | ------- | ------- | ------- |
| view_analysis   | &check; | &check; | &check; | &check; |
| run_analysis    |         | &check; | &check; | &check; |
| remove_analysis |         | &check; | &check; | &check; |

### Token management

| Permission            | Admin | Manager | Analyst | Auditor |
| --------------------- | ----- | ------- | ------- | ------- |
| create_analysis_token |       |         | &check; | &check; |
| delete_analysis_token |       |         | &check; | &check; |
