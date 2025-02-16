docker build . -t clustache:latest

# Restart the Kubernetes deployment
# kubectl rollout restart deployment clustache