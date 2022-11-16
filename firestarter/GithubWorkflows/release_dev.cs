namespace firestarter.GithubWorkflows;

public static class release_dev
{
    public static string content (List<string> projectNames) => $@"name: Release dev [DEV02]

on:
  push:
    branches:
      - dev
jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projectNames.Select(x=>($@"release-{x}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev02
      prefix: dev
      cluster: autoproff-cluster
      service_name: {x}-service
      dockerfile: ""{x}-dockerfile""
      container_name: dev-{x}-container")))}
";
}