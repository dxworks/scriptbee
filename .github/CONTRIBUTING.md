# Contributing to ScriptBee

Welcome! We are excited that you want to contribute to ScriptBee. This document outlines the guidelines and best practices for contributing to the project.

## How to Contribute

### Reporting Bugs
If you find a bug, please check if an issue already exists. If not, open a new issue using the **Bug Report** template for the relevant component (WebApp, Client, Plugins).

### Suggesting Enhancements
We welcome ideas for new features or improvements. Please open an issue using the **Feature Request** template to discuss your ideas with the maintainers.

### Pull Requests
1. **Fork the repository** and create your branch from `main`.
2. **Implement changes** following the coding standards described below.
3. **Ensure tests pass** and add new tests if necessary.
4. **Write conventional commit messages** (see below).
5. **Submit a Pull Request** with a clear description of the changes and link any related issues.

## Project Structure

- **ScriptBeeWebApp**: The backend server built with C# and .NET.
- **ScriptBeeClient**: The frontend application built with Angular and TypeScript.
- **Plugins**: Community and core plugins for ScriptBee.
- **DxWorks.ScriptBee.Plugin.Api**: The API used for building plugins.
- **docs**: Documentation built with MkDocs.

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
- Documentation is located in the [docs](../docs) directory and configured via `mkdocs.yml`.
- Write clear, concise, and professional English.

## Commit Messages

We use **[Conventional Commits](https://www.conventionalcommits.org/)** for all commit messages. This keeps the Git history clean and enables automated tooling.

**Format:** `<type>(<scope>): <description>`

Common types:

| Type | When to use |
|------|-------------|
| `feat` | A new feature |
| `fix` | A bug fix |
| `docs` | Documentation changes only |
| `refactor` | Code change that is not a fix or feature |
| `test` | Adding or updating tests |
| `chore` | Build process, tooling, dependencies |

**Examples:**
```
feat: add dark mode toggle
fix: resolve project loading race condition
docs: update plugin API reference
```

## Development Setup

- **Backend**: Requires .NET 10 or higher. Open `ScriptBee.slnx` or the individual project folders.
- **Frontend**: Requires Node.js and npm. Navigate to `ScriptBeeClient` and run `npm install`.
- **Documentation**: Requires Python and MkDocs. Run `pip install -r docs/requirements.txt` (if applicable) or `mkdocs serve`.

Thank you for your contributions!
