namespace firestarter.Templates;

public static class TemplateClass
{
    public static string NamingReleaseStep(Project s) => $"release-{s.ServiceName}";
    public static string WaitUntilStable(SolutionDescription solution, DeploymentEnvironments env)
    {
        return string.Join(Environment.NewLine + Environment.NewLine + "  ",solution.Projects.Select(x => $@"wait-until-stable-{x.ServiceName}:
    secrets: inherit
    needs: [{NamingReleaseStep(x)}]
    uses: ./.github/workflows/wait-until-stable.yml
    with:
      environment: {env + (solution.GitWorkflow == GitWorkflow.Gitflow?"02":"")}
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
      environment: {env + (solution.GitWorkflow == GitWorkflow.Gitflow?"02":"")}
      prefix: {env}
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      branch_name: {(solution.GitWorkflow == GitWorkflow.Gitflow?env.ToBranch():DeploymentEnvironments.prod.ToBranch())}
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: {env}-{x.LegacyProperties.ContainerName}" : "")}"
            )));
    }
}