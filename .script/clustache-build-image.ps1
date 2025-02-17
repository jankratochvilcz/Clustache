$currentDirectory = Get-Location
$cacheServerDockerfile = Join-Path $currentDirectory "CacheServer.dockerfile"
$databaseMockDockerfile = Join-Path $currentDirectory "DatabaseMock.dockerfile"

docker build -f $cacheServerDockerfile -t clustache:latest .
docker build -f $databaseMockDockerfile -t clustache-mock-database:latest .