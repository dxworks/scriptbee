# ScriptBee Kubernetes Quickstart

This directory contains ready-to-use Kubernetes manifests for deploying ScriptBee with the native Kubernetes analysis runner driver (`SCRIPTBEE__ANALYSIS__DRIVER=kubernetes`).

## Prerequisites

- A running Kubernetes cluster (e.g. [k3d](https://k3d.io/), [Minikube](https://minikube.sigs.k8s.io/), [Kind](https://kind.sigs.k8s.io/), [Docker Desktop Kubernetes](https://docs.docker.com/desktop/features/kubernetes/), or a remote cluster).
- `kubectl` configured to connect to your cluster.
- A default StorageClass configured in your cluster (standard in Minikube, Docker Desktop, k3d, and cloud providers) to fulfill the `PersistentVolumeClaim` requests.

---

## 1. Base Setup (Development Mode)

The base setup runs MongoDB and ScriptBee Gateway with authentication disabled (`AUTHENTICATION__AUTHMODE=Development`).

### Start

Apply all manifests in the `base` directory:

```bash
kubectl apply -f quickstart/kubernetes/base
```

Verify that all pods are running in the `scriptbee` namespace:

```bash
kubectl get pods -n scriptbee
```

### Access ScriptBee

Forward the ScriptBee Gateway port to your local machine:

```bash
kubectl port-forward -n scriptbee svc/scriptbee 4201:80
```

Open your browser and navigate to:
**[http://localhost:4201](http://localhost:4201)**

When you run an analysis, ScriptBee Gateway will dynamically spawn an Analysis Pod and companion ClusterIP Service in the `scriptbee` namespace.

### Stop / Teardown

```bash
kubectl delete -f quickstart/kubernetes/base
```

---

## 2. Full Setup (with Keycloak & OPA)

The full setup includes MongoDB, Open Policy Agent (OPA) with RBAC policies, Keycloak for identity and authentication, and ScriptBee Gateway.

### Start

Apply all manifests in the `full` directory:

```bash
kubectl apply -f quickstart/kubernetes/full
```

Wait for all pods to be in `Running` state:

```bash
kubectl get pods -n scriptbee
```

### Access ScriptBee & Keycloak

Forward ports for both the Gateway and Keycloak:

**Terminal 1 (ScriptBee Gateway):**
```bash
kubectl port-forward -n scriptbee svc/scriptbee 4201:80
```

**Terminal 2 (Keycloak):**
```bash
kubectl port-forward -n scriptbee svc/keycloak 8082:8080
```

Open your browser and navigate to:
**[http://localhost:4201](http://localhost:4201)**

Log in using the pre-configured credentials:
- **Username:** `test`
- **Password:** `test`

### Stop / Teardown

```bash
kubectl delete -f quickstart/kubernetes/full
```
