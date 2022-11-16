namespace firestarter.GithubWorkflows;

public static class release_qa
{
    public static string content(List<string> projectNames) => $@"name: Release QA [STAGE02]      

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
  checkout:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v3
        with:
          ref: release
  
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projectNames.Select(x=>($@"release-{x}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: stage02
      prefix: stage
      cluster: autoproff-cluster
      service_name: {x}-service
      dockerfile: ""{x}-dockerfile""
      container_name: stage-{x}-container")))}
";
}