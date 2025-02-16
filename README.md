# Clustache

The core idea here is to use Redis pub/sub to sync in-memory cache within pods on a single Kubernetes cluster.

## Local Setup

Make sure you have the following installed:
* Docker desktop - local Kubernetes cluster is enabled
* Kubectl
* Helm
* (optional) Helm - UI for Kuberbetes
* VS Code

You have these tasks configured in VS Code. Run them in order.
* `Clustache: Build image` - Builds images for .NET projects and adds them to local repository
* `Clustache: Clustache: Ship chart` - Build Helm chart and applies them to cluster

When this is done, forward port (Lens has easy UI for this) for Grafana and open it in browser (user + password is admin for both).
Next, add source (Connections -> Data sources), pick Prometheus, and for URL add `http://clustache-v0-prometheus-server:80`.

You should be set. The k6 job is running and metrics should be collected.

Create a new dashboard and add the below queries, you should see data.

```
// Cache fill rate
(sum(clustache_cache_filled_from_cache_subscription) + sum(clustache_cache_filled_from_cache_own)) / (sum(clustache_cache_filled_from_cache_subscription) + sum(clustache_cache_filled_from_cache_own) + sum(clustache_cache_filled_from_source))

// % of cached items coming from pub/sub
sum(clustache_cache_filled_from_cache_subscription) / (sum(clustache_cache_filled_from_cache_subscription) + sum(clustache_cache_filled_from_cache_own))
```