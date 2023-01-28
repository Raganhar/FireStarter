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

  {string.Join(Environment.NewLine + Environment.NewLine + "  ",solution.Projects.Where(x=>x.MigrationUtils!=null).Select(x => $@"run-database-migrations-{x.ServiceName}:
    secrets: inherit
    needs: [release-product-service]
    uses: ./.github/workflows/run-migration-task.yml
    with:
      environment: {(solution.GitWorkflow == GitWorkflow.Gitflow?"prod02":"prod")}
      prefix: prod
      service_name: ""{x.ServiceName}""
      security_groups: ""{x.MigrationUtils.SecurityConfig_prod.Security_groups}""
      subnets: ""{x.MigrationUtils.SecurityConfig_prod.Subnets}""
      db_assembly: ""{x.MigrationUtils.Db_assembly}""
      db_database: ""{x.MigrationUtils.Db_database}""
      db_commandtype: ""databasemigrationup""
      db_context_type: ""{x.MigrationUtils.Db_context_type}""
"))}

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