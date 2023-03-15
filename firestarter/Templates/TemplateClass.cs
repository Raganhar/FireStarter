namespace firestarter.Templates;

public static class TemplateClass
{
    public static string CreateNugetConfig() => @"
    - name: Create nuget file
      run: dotnet new nugetconfig
    - name: Set nuget auth to github
      run: | 
        dotnet nuget add source https://nuget.pkg.github.com/AUTOProff/index.json \ 
          -n github \
          -u ${{ secrets.PACKAGE_REGISTRY_USER }} \
          -p ${{ secrets.PACKAGE_REGISTRY_READ_TOKEN }} \
          --configfile nuget.config \
          --store-password-in-clear-text
";
    public static string NamingReleaseStep(Project s) => $"release-{s.ServiceName}";
    public static string WaitUntilStable(SolutionDescription solution, DeploymentEnvironments env)
    {
        // return ""; // this is apparently too expensive... since its billed as normal executing time
        var shouldBeWaited = new List<DeploymentEnvironments>() { DeploymentEnvironments.prod ,DeploymentEnvironments.stage}.Contains(env);
        if (!shouldBeWaited)
        {
            return "";
        }
        return string.Join(Environment.NewLine + Environment.NewLine + "  ",solution.Projects.Select(x => $@"wait-until-stable-{x.ServiceName}:
    secrets: inherit
    needs: [{NamingReleaseStep(x)}]
    uses: ./.github/workflows/wait-until-stable.yml
    with:
      environment: {DetermineEnvironment(solution, env)}
      prefix: {env}
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      "));
    }
    public static string ReleaseEcs(SolutionDescription solution, DeploymentEnvironments env)
    {
        return string.Join(Environment.NewLine+Environment.NewLine+"  ",solution.Projects.Select(x=>($@"{TemplateClass.NamingReleaseStep(x)}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: {DetermineEnvironment(solution, env)}
      prefix: {env}
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      branch_name: {(solution.GitWorkflow == GitWorkflow.Gitflow?env.ToBranch():DeploymentEnvironments.prod.ToBranch())}
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: {env}-{x.LegacyProperties.ContainerName}" : "")}"
            )));
    }
    
    public static string RunMigrationUtil(SolutionDescription solution, DeploymentEnvironments env)
    {
        return string.Join(Environment.NewLine + Environment.NewLine + "  ",solution.Projects.Where(x=>x.MigrationUtils!=null).Select(x => $@"run-database-migrations-{x.ServiceName}:
    secrets: inherit
    needs: [release-product-service]
    uses: ./.github/workflows/run-migration-task.yml
    with:
      environment: {DetermineEnvironment(solution, env)}
      prefix: {env}
      service_name: ""{x.ServiceName}""
      db_assembly: ""{x.MigrationUtils.Db_assembly}""
      db_database: ""{x.MigrationUtils.Db_database}""
      db_commandtype: ""databasemigrationup""
      db_context_type: ""{x.MigrationUtils.Db_context_type}""
"));
    }

    private static string DetermineEnvironment(SolutionDescription solution, DeploymentEnvironments env)
    {
        if (env == DeploymentEnvironments.dev03)
        {
            return env.ToString();
        }
        return env + (solution.GitWorkflow == GitWorkflow.Gitflow?"02":"");
    }
}