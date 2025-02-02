# Overview

## UI

The UI uses plugins defined by the gateway.

The plugins can be of several types

### Output Type plugin

To offer different visualization of the data

## Gateway

Is responsible for the interaction with UI, user authentication and authorization, project management, communicating
with the calculation instances

### Persistence

Data is stored in MongoDB

## Calculation

Is responsible for loading data, linking the models and executing scripts to obtain different results

Calculation can be done in 2 types of instances:

- Permanent
- Temporary

A permanent instance is allocated and deallocated by the user on command. The purpose is to have a long-running instance
with the linked model to run different types of analysis

A temporary instance is used for one time processing of the models. The model is loaded and linked, a set of predefined
scripts is run to perform the analysis. After the calculations are done, the instance is deallocated. It is used for
CI/CD analysis

### Persistence

Data is stored in MongoDB

## Deployment

See [Features](features.md) for more information about the deployment

### Kubernetes

The deployment in Kubernetes is done deploying the calculation engine via a CRD

### Docker

The deployment is done using Docker API
