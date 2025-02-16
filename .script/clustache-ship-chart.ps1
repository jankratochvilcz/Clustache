$releaseName = "clustache-v0"

$releaseExists = helm list -q | Select-String -Pattern $releaseName

if ($releaseExists) {
    # Upgrade the existing release
    helm upgrade $releaseName ./clustache-chart
} else {
    # Install a new release
    helm install $releaseName ./clustache-chart
}