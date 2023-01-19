namespace firestarter.GithubWorkflows;

public static class release_preprod
{
    public static string content(List<Project> projects) => $@"name: Release preprod [DEV03]    

on:
  push:
    tags:
      - main.**
  workflow_dispatch:

jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projects.Select(x=>($@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev03
      prefix: dev03
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      branch_name: main
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: dev03-{x.LegacyProperties.ContainerName}" : "")}"
    )))}

  {string.Join(Environment.NewLine + Environment.NewLine + "  ",projects.Where(x=>x.MigrationUtils!=null).Select(x => $@"run-database-migrations-{x.ServiceName}:
    secrets: inherit
    needs: [release-product-service]
    uses: ./.github/workflows/run-migration-task.yml
    with:
      environment: dev03
      prefix: dev03
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