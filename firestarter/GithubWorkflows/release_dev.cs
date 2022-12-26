namespace firestarter.GithubWorkflows;

public static class release_dev
{
    public static string content (List<Project> projects) => $@"name: Release dev [DEV02]

on:
  create:
    tags:
      - 'dev.*'
  workflow_dispatch:

jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projects.Select(x=>($@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev02
      prefix: dev
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      dockerfile: ""{x.DockerFile}""
      branch_name: dev
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: dev-{x.LegacyProperties.ContainerName}" : "")}"
    )))}
";
}