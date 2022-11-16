namespace firestarter.GithubWorkflows;

public static class release_qa
{
    public static string content(List<Project> projects) => $@"name: Release QA [STAGE02]      

on:
  workflow_run:
    workflows:
      - Promote dev
    types:
      - completed
  push:
    branches:
      - ""release""
jobs:
  {string.Join(Environment.NewLine + Environment.NewLine + "  ", projects.Select(x => $@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: stage02
      prefix: stage
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      dockerfile: ""{x.DockerFile}""
      branch_name: release
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: stage-{x.LegacyProperties.ContainerName}" : "")}"
      ))
  }
";
}