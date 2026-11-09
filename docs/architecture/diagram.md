# Architecture Diagram

![architecture_diagram](../assets/ScriptBeeArchitecture.svg 'Architecture Diagram')

The OIDC provider can be any provider that supports the OpenID Connect protocol, such as Azure AD, Auth0, or Keycloak.

The OPA server is responsible for enforcing the RBAC policies defined in the OPA rules file. The OPA server can be
configured to use different data sources for user roles and permissions, such as a database or an external identity
provider.

See [Authentication](https://dxworks.org/scriptbee/architecture/configuration/gateway_configuration.html#authentication) for more information on how to configure the OPA server.

## Kubernetes Analysis Instance Lifecycle

```mermaid
sequenceDiagram
    autonumber
    participant Gateway as ScriptBee Gateway
    participant K8s as Kubernetes API
    participant Pod as Analysis Pod
    participant Service as ClusterIP Service

    Gateway->>K8s: Create Pod (scriptbee-analysis-{id}) with ReadinessProbe (/health)
    Gateway->>K8s: Create ClusterIP Service (scriptbee-analysis-{id})
    K8s->>Pod: Schedule, pull image, start container
    Pod-->>K8s: /health 200 OK (Readiness = True)
    K8s->>Service: Direct traffic to pod selector (instance-id)
    Gateway->>Gateway: Return FQDN: http://scriptbee-analysis-{id}.{namespace}.svc.cluster.local:{port}
    Gateway->>K8s: Poll GetStatus (Pending -> Allocating, Ready -> Running)
    Gateway->>Pod: Dispatch REST commands via Service FQDN
    Gateway->>K8s: Deallocate: Delete Pod & Service
```
