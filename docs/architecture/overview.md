# Overview

This section provides a high-level overview of ScriptBee's architecture and the roles of its core components.

## UI

The User Interface provides a web-based experience for managing projects, loading data from various sources, and running analysis scripts.

## Gateway

The Gateway is the central orchestration component. Its responsibilities include:

- Handling UI interactions and requests.
- Managing projects and user workspaces.
- Orchestrating the lifecycle of analysis instances.

> [!NOTE]
> **Authentication & Authorization** are handled by a separate, external service. The Gateway integrates with this service to manage project access.

## Analysis

Analysis is responsible for the core logic of processing models and executing scripts. It performs the following functions:

- **Data Loading**: Ingesting data from various sources using loader plugins.
- **Model Linking**: Connecting loaded models using linker plugins to create a unified data structure.
- **Script Execution**: Running C#, Javascript, or Python scripts against the linked models.

Analysis can be performed in two modes:

- **Permanent Instances**: Manually managed by the user for long-running exploration and interactive analysis.
- **Temporary Instances**: Automatically created for one-time processing, ideal for automated pipelines or quick checks.

## Persistence

Both the **Gateway** and **Analysis** services rely on a MongoDB database for data persistence:

- **Gateway**: Stores project configurations, workspace metadata, and instance statuses.
- **Analysis**: Uses the database for temporary data storage during processing and for persisting analysis results.

## Deployment & Orchestration

The orchestration of analysis components varies based on the environment:

### Docker

In standard containerized environments, a **Docker Executor** manages analysis instances dynamically. It handles:

- **Networking**: Ensuring the Gateway can communicate with the analysis instance.
- **Data Access**: Mapping local data folders to the analysis container so loaders and scripts can access source files.

See [Docker Compose](../installation/docker_installation.md) for more information about deployment to Docker

### Kubernetes

In Kubernetes deployments, analysis is managed through specialized controllers that handle the lifecycle of each instance as a native resource.

See [Kubernetes](../installation/kubernetes_installation.md) for more information about deployment to Kubernetes
