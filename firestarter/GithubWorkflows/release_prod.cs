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

  transition-jira-issues-on-trigger:
    runs-on: ubuntu-latest
    steps:
      - name: transition jira tickets
        uses: AUTOProff/ap-github-action-jira-transition@v1
        env:
          GITHUB_CONTEXT: ""${{{{ toJson(github) }}}}""
        with:
          jira-api-key: ${{{{ secrets.JIRA_API_TOKEN }}}}
          jira-url: ${{{{ secrets.JIRA_BASE_URL }}}}
          jira-user: ${{{{ secrets.JIRA_USER_EMAIL }}}}
          main-jira-transition: done
          release-jira-transition: in progress
          branch_to_compare_to: main
          jira_state_when_revert: blocked

  {TemplateClass.WaitUntilStable(solution, DeploymentEnvironments.prod)}
";
}