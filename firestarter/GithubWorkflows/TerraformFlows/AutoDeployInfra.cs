namespace firestarter.GithubWorkflows.TerraformFlows;

public static class AutoDeployInfra
{
    public static string content(SolutionDescription solution) => $@"
name: Deploy Infrastructure

on:
  push:
    branches:
      - main
  workflow_dispatch:

jobs:
  wait-for-verify:
    runs-on: ubuntu-latest
    steps:
      - name: Wait for tests to succeed
        uses: lewagon/wait-on-check-action@v1.2.0
        with:
          ref: main
          check-name: verify
          repo-token: ${{{{ secrets.GITHUB_TOKEN }}}}
          wait-interval: 20

  deploy-environment-stage:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: {DeploymentEnvironments.stage}
      profile: stage02
      workspace: {solution.Projects.First().TerraformWorkSpace(DeploymentEnvironments.stage)}

  deploy-environment-dev:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: {DeploymentEnvironments.dev}
      profile: dev02
      workspace: {solution.Projects.First().TerraformWorkSpace(DeploymentEnvironments.dev)}

  deploy-environment-prod:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: {DeploymentEnvironments.prod}
      profile: prod02
      workspace: {solution.Projects.First().TerraformWorkSpace(DeploymentEnvironments.prod)}
";
}