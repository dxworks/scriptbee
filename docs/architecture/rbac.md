# RBAC Configuration

> [!IMPORTANT]
> Role-Based Access Control (RBAC) and Authentication features are handled by a **separate, external service**. The
> implementation details here represent how ScriptBee interacts with that service. The implementation is not yet fully
> finalized and may be subject to change.

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
