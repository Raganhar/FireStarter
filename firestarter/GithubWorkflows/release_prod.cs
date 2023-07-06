using firestarter.Templates;

namespace firestarter.GithubWorkflows;

public static class release_prod
{
    public static string content(SolutionDescription solution) => $@"name: Release prod [PROD02]

on:
  workflow_dispatch:
  {(solution.GitWorkflow == GitWorkflow.Gitflow?"":@"push:
    tags:
      - main.**")}

jobs:
  {TemplateClass.ReleaseEcs(solution, DeploymentEnvironments.prod)}

  {TemplateClass.RunMigrationUtil(solution,DeploymentEnvironments.prod)}

  {TemplateClass.WaitUntilStable(solution, DeploymentEnvironments.prod)}
";
}