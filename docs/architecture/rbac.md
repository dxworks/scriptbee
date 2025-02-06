# RBAC

Roles:

- Guest - basic user with limited capabilities
- Analyst - user that can perform different analysis on allowed projects
- Maintainer - handles operations on the allowed projects
- Administrator - has full access for every functionality

## Project Management

| Permission     | Guest   | Analyst | Maintainer | Administrator |
|----------------|---------|---------|------------|---------------|
| view_project   | &check; | &check; | &check;    | &check;       |
| update_project |         |         | &check;    | &check;       |
| delete_project |         |         | &check;    | &check;       |
| create_project |         |         |            | &check;       |

## User Management

| Permission          | Guest | Analyst | Maintainer | Administrator |
|---------------------|-------|---------|------------|---------------|
| update_project_user |       |         | &check;    | &check;       |
| update_user_role    |       |         |            | &check;       |

## Model Management

| Permission       | Guest | Analyst | Maintainer | Administrator |
|------------------|-------|---------|------------|---------------|
| install_loader   |       | &check; | &check;    | &check;       |
| uninstall_loader |       | &check; | &check;    | &check;       |
| install_linker   |       | &check; | &check;    | &check;       |
| uninstall_linker |       | &check; | &check;    | &check;       |

## Analysis

| Permission            | Guest   | Analyst | Maintainer | Administrator |
|-----------------------|---------|---------|------------|---------------|
| view_analysis         | &check; | &check; | &check;    | &check;       |
| run_analysis          |         | &check; | &check;    | &check;       |
| remove_analysis       |         | &check; | &check;    | &check;       |
| view_script           |         | &check; | &check;    | &check;       |
| create_script         |         | &check; | &check;    | &check;       |
| update_script         |         | &check; | &check;    | &check;       |
| delete_script         |         | &check; | &check;    | &check;       |
| create_analysis_token |         |         | &check;    | &check;       |
| delete_analysis_token |         |         | &check;    | &check;       |
