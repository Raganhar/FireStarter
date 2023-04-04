using firestarter.Templates;

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

  {TemplateClass.ReleaseEcs(solution,DeploymentEnvironments.dev)}

  {TemplateClass.RunMigrationUtil(solution,DeploymentEnvironments.dev)}

  {TemplateClass.WaitUntilStable(solution, DeploymentEnvironments.dev)}
";
}