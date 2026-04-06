# Contributing to ScriptBee

Welcome! We are excited that you want to contribute to ScriptBee. This document outlines the guidelines and best
practices for contributing to the project.

## How to Contribute

### Reporting Bugs

If you find a bug, please check if an issue already exists. If not, open a new issue using the **Bug Report** template
for the relevant component (WebApp, Client, Plugins).

### Suggesting Enhancements

We welcome ideas for new features or improvements. Please open an issue using the **Feature Request** template to
discuss your ideas with the maintainers.

### Pull Requests

1. **Fork the repository** and create your branch from `main`.
2. **Implement changes** following the coding standards described below.
3. **Ensure tests pass** and add new tests if necessary.
4. **Write conventional commit messages** (see below).
5. **Submit a Pull Request** with a clear description of the changes and link any related issues.

## Repository Structure

To help you navigate the project, here is a breakdown of the key directories:

- **`DxWorks.ScriptBee.Plugin.Api/`**: The core library containing the interfaces required for developing ScriptBee
  plugins (e.g., `IModelLoader`, `IModelLinker`, `IScriptRunner`).
- **`Plugins/`**: Contains the default plugin implementations and helper functions.
    - `HelperFunctions/`: Default helper functions for script execution.
    - `ScriptRunner/`: Default runners for C#, Javascript, and Python scripts.
- **`ScriptBeeClient/`**: The Angular-based frontend application (UI).
- **`ScriptBeeWebApp/`**: The main backend service implemented in .NET. It includes:
    - `src/Gateway`: Orchestration, project management, and instance allocation.
    - `src/Analysis`: Core logic for model loading and script execution.
    - `src/Workspace`: Model persistence and management.
- **`docs/`**: Documentation source files, built with **VitePress**.
- **`scripts/`**: Utility scripts for building, packing, and maintaining the project.

## Coding Standards

### Backend (C#)

- Follow the rules defined in [.editorconfig](../.editorconfig).
- We use **CSharpier** for automatic code formatting.
- Ensure all public APIs are documented.

### Frontend (TypeScript / Angular)

- We use **Prettier** for formatting.
- We use **ESLint** for linting.
- Components should follow the Angular Style Guide.

### Documentation

- Documentation is located in the [docs](../docs) directory and built with **VitePress**.
- Run the documentation locally:
  ```bash
  cd docs
  npm install
  npm run docs:dev
  ```

## Commit Messages

We use **[Conventional Commits](https://www.conventionalcommits.org/)** for all commit messages.

**Format:** `<type>(<scope>): <description>`

Common types: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`.

## Development Setup

- **Backend**: Requires .NET 10 or higher. Open `ScriptBee.slnx` or the individual project folders.
- **Frontend**: Requires Node.js and npm. Navigate to `ScriptBeeClient` and run `npm install`.
- **Docker**: Many components rely on Docker for container orchestration during development (e.g., running MongoDB or
  Analysis instances).

Thank you for your contributions!
