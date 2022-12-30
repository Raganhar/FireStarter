namespace firestarter.GithubWorkflows;

public static class release_qa
{
    public static string content(SolutionDescription solution) => $@"name: Release QA [STAGE02]      

on:
  push:
    tags:
      - {(solution.GitWorkflow == GitWorkflow.Gitflow?"release":"main")}.**
  workflow_dispatch:

jobs:
  {string.Join(Environment.NewLine + Environment.NewLine + "  ",solution.Projects.Select(x => $@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: {(solution.GitWorkflow == GitWorkflow.Gitflow?"stage02":"stage")}
      prefix: stage
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      branch_name: {(solution.GitWorkflow == GitWorkflow.Gitflow?"release":"main")}
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: stage-{x.LegacyProperties.ContainerName}" : "")}"
      ))
  }
";
}