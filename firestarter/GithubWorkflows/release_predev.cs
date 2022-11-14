namespace firestarter.GithubWorkflows;

public static class release_predev
{
    public static string content(List<string> projectNames) => $@"name: Release predev [DEV03]    

on:
  push:
    branches:
      - main

  workflow_dispatch:

jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projectNames.Select(x=>($@"release-{x}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev03
      prefix: dev03
      cluster: {x}-cluster
      service_name: {x}-service
      dockerfile: ""{x}-dockerfile""
      container_name: dev03-{x}-container")))}
";
}