call firestarter.exe with optional directory which contains '.ap' folder which should contain a solution.json

example solutionDescription

```{
  "Projects": [
    {
      "Name": "auction",
      "Tech": "legacy_dotnet"
    },
    {
      "Name": "auction-alb",
      "Tech": "legacy_dotnet",
      "LegacyProperties":{
        "ServiceName":"auction-service-alb",
        "ContainerName":"auction-service"
      }
    },
    {
      "Name": "auction",
      "Tech": "legacy_dotnet",
      "LegacyProperties":{
        "DockerFile":"DockerfileBackground",
        "ServiceName":"auction-service-worker",
        "ContainerName":"auction-service-background-worker"
      }
    }
  ],
  "LegacySystem":"auction_service",
  "Version": "v1"
}

