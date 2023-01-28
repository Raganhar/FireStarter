namespace firestarter.GithubWorkflows;

public static class release_preprod
{
    public static string content(List<Project> projects) => $@"name: Release preprod [DEV03]    

on:
  push:
    tags:
      - main.**
  workflow_dispatch:

jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projects.Select(x=>($@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev03
      prefix: dev03
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      branch_name: main
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: dev03-{x.LegacyProperties.ContainerName}" : "")}"
    )))}
";
}