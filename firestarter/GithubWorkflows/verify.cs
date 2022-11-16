namespace firestarter.GithubWorkflows;

public static class verify
{
  public static string content(List<Project> projects) => $@"name: verify

on:
  pull_request:
jobs:
  verify:
    runs-on: ubuntu-latest
    steps:
      - name: ""Checkout""
        uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '{(projects.GroupBy(c => c.Tech).Count() == 1 && projects.GroupBy(c => c.Tech).First().Key == TechStack.legacy_dotnet ? "3" : "6")}.x'
      - run: dotnet restore
      - run: dotnet test {(!string.IsNullOrWhiteSpace(projects.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.TestFilter))?.TestFilter) ? $"--filter {projects.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.TestFilter))?.TestFilter}" : "")}
";
}