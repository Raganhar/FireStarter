using firestarter.Templates;

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
  {TemplateClass.ReleaseEcs(solution,DeploymentEnvironments.stage)}

  {TemplateClass.RunMigrationUtil(solution,DeploymentEnvironments.stage)}

  {TemplateClass.WaitUntilStable(solution, DeploymentEnvironments.stage)}
";
}