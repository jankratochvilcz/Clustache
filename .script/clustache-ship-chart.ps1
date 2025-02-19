$releaseName = "clustache-v0"

$releaseExists = helm list -q | Select-String -Pattern $releaseName

if ($releaseExists) {
    # Upgrade the existing release
    helm upgrade $releaseName ./clustache-chart

    # Forces pods cycling
    kubectl delete pods -l app=clustache
    kubectl delete pods -l app=clustache-mock-database
} else {
    # Install a new release
    helm install $releaseName ./clustache-chart
}