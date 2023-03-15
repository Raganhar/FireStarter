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
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '{(solution.Projects.GroupBy(c => c.Tech).Count() == 1 && solution.Projects.GroupBy(c => c.Tech).First().Key == TechStack.legacy_dotnet ? "3" : "6")}.x'

    - name: Create nuget file
      run: dotnet new nugetconfigfile
    - name: Set nuget auth to github
      run: dotnet nuget add source https://nuget.pkg.github.com/AUTOProff/index.json \ 
          -n github \
          -u ${{{{ secrets.PACKAGE_REGISTRY_USER }}}} \
          -p ${{{{ secrets.PACKAGE_REGISTRY_READ_TOKEN }}}} \
          --configfile nuget.config \
          --store-password-in-clear-text
  
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --no-restore --filter Category=SanityTest --verbosity normal -l:""trx;LogFileName=testresult.xml""
      - name: Test Report
        uses: dorny/test-reporter@v1 #you need to include nuget package: coverlet.collector in your test project
        if: success() || failure()    # run this step even if previous step failed
        with:
          name: Test results            # Name of the check run which will be created
          path:  '*/TestResults/*.xml'     # Path to test results
          reporter: dotnet-trx        # Format of test result
";
}