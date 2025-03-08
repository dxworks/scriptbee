# Features

> Work in Progress

## Deployment

> All below environment variables are subject to change

### Kubernetes

For the Kubernetes deployment, the following environment variables need to be set

```dotenv
FEATURES__DEPLOYMENT_CALCULATION=kubernetes
```

### Docker Swarm

For the Docker swarm deployment, the following environment variables need to be set

```dotenv
FEATURES__DEPLOYMENT_CALCULATION=docker
```

## Authorization

Authorization can be disabled entirely

> Not recommended for production

```dotenv
FEATURES_DISABLEAUTHORIZATION=true
```
