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

  {string.Join(Environment.NewLine + Environment.NewLine + "  ",solution.Projects.Where(x=>x.MigrationUtils!=null).Select(x => $@"run-database-migrations-{x.ServiceName}:
    secrets: inherit
    needs: [release-product-service]
    uses: ./.github/workflows/run-migration-task.yml
    with:
      environment: {(solution.GitWorkflow == GitWorkflow.Gitflow?"dev02":"dev")}
      prefix: dev
      service_name: ""{x.ServiceName}""
      security_groups: ""{x.MigrationUtils.SecurityConfig_dev.Security_groups}""
      subnets: ""{x.MigrationUtils.SecurityConfig_dev.Subnets}""
      db_assembly: ""{x.MigrationUtils.Db_assembly}""
      db_database: ""{x.MigrationUtils.Db_database}""
      db_commandtype: ""databasemigrationup""
      db_context_type: ""{x.MigrationUtils.Db_context_type}""
"))}

  {TemplateClass.WaitUntilStable(solution, DeploymentEnvironments.dev)}
";

    
}