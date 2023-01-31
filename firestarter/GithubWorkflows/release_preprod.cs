using firestarter.Templates;

namespace firestarter.GithubWorkflows;

public static class release_preprod
{
    public static string content(SolutionDescription solution) => $@"name: Release preprod [DEV03]    

on:
  push:
    tags:
      - main.**
  workflow_dispatch:

jobs:
  {TemplateClass.ReleaseEcs(solution,DeploymentEnvironments.dev03)}

  {TemplateClass.RunMigrationUtil(solution,DeploymentEnvironments.dev03)}

  {TemplateClass.WaitUntilStable(solution, DeploymentEnvironments.dev03)}
";
}