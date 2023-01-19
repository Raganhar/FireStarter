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
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",solution.Projects.Select(x=>($@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: {(solution.GitWorkflow == GitWorkflow.Gitflow?"dev02":"dev")}
      prefix: dev
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      branch_name: {(solution.GitWorkflow == GitWorkflow.Gitflow?"dev":"main")}
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: dev-{x.LegacyProperties.ContainerName}" : "")}"
    )))}

  {string.Join(Environment.NewLine + Environment.NewLine + "  ",solution.Projects.Where(x=>x.MigrationUtils!=null).Select(x => $@"run-database-migrations-{x.ServiceName}:
    secrets: inherit
    needs: [release-product-service]
    uses: ./.github/workflows/run-migration-task.yml
    with:
      environment: {(solution.GitWorkflow == GitWorkflow.Gitflow?"dev02":"dev")}
      prefix: stage
      service_name: ""{x.ServiceName}""
      security_groups: ""{x.MigrationUtils.Security_groups}""
      subnets: ""{x.MigrationUtils.Subnets}""
      db_assembly: ""{x.MigrationUtils.Db_assembly}""
      db_database: ""{x.MigrationUtils.Db_database}""
      db_commandtype: ""databasemigrationup""
      db_context_type: ""{x.MigrationUtils.Db_context_type}""
"))}

";
}