# Clustache

The core idea here is to use Redis pub/sub to sync in-memory cache within pods on a single Kubernetes cluster.

## Local Setup

### Metrics

You'll need Prometheus and Grafana. Install this to your cluster via Helm.

```bash
helm install prometheus prometheus-community/prometheus
helm install grafana grafana/grafana --set adminPassword='admin' --set service.type=LoadBalancer
```

## Hot Commands

```bash
docker build . -t clustache:latest
kubectl rollout restart deployment clustache

kubectl delete job k6-load-test
kubectl apply -f k8s/k6/job.yaml
```

## Grafana Queries

```
// Cache fill rate
(sum(clustache_cache_filled_from_cache_subscription) + sum(clustache_cache_filled_from_cache_own)) / (sum(clustache_cache_filled_from_cache_subscription) + sum(clustache_cache_filled_from_cache_own) + sum(clustache_cache_filled_from_source))

// % of cached items coming from pub/sub
sum(clustache_cache_filled_from_cache_subscription) / (sum(clustache_cache_filled_from_cache_subscription) + sum(clustache_cache_filled_from_cache_own))
```