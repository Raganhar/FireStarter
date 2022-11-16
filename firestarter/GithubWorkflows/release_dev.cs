namespace firestarter.GithubWorkflows;

public static class release_dev
{
    public static string content (List<Project> projects) => $@"name: Release dev [DEV02]

on:
  push:
    branches:
      - dev
jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projects.Select(x=>($@"release-{x.Name}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev02
      prefix: dev
      cluster: autoproff-cluster
      service_name: {x.Name}-service
      dockerfile: ""{x.DockerFile}""
      {(!string.IsNullOrWhiteSpace(x.ContainerName) ? $"container_name: dev-{x.ContainerName}" : "")}"
    )))}
";
}