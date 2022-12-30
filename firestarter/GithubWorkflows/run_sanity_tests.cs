namespace firestarter.GithubWorkflows;

public static class run_sanity_tests
{
  public static string content(SolutionDescription solution) => $@"
name: run sanity tests

on:
  workflow_run:
    workflows:
      - Release QA \[STAGE02\]
    types:
      - completed

jobs:
  sanity_tests:
    runs-on: ubuntu-latest
    steps:
      - name: ""Checkout""
        uses: actions/checkout@v3
        with:
          ref: {(solution.GitWorkflow == GitWorkflow.Gitflow?"release":"main")} 
      - uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '{(solution.Projects.GroupBy(c => c.Tech).Count() == 1 && solution.Projects.GroupBy(c => c.Tech).First().Key == TechStack.legacy_dotnet ? "3" : "6")}.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --no-restore --filter Category=SanityTest
";
}