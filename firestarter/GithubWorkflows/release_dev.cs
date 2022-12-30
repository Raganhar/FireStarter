namespace firestarter.GithubWorkflows;

public static class release_dev
{
    public static string content (SolutionDescription solution) => $@"name: Release dev [DEV02]

on:
  push:
    tags:
      - {(solution.GitWorkflow == GitWorkflow.Gitflow?"dev":"main")}.**
  workflow_dispatch:

jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",solution.Projects.Select(x=>($@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: {(solution.GitWorkflow == GitWorkflow.Gitflow?"dev02":"dev")}
      prefix: dev
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      branch_name: {(solution.GitWorkflow == GitWorkflow.Gitflow?"dev":"main")}
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: dev-{x.LegacyProperties.ContainerName}" : "")}"
    )))}
";
}