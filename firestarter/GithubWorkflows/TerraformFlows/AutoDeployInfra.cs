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
      environment: stage
      profile: stage02
      workspace: stage-{solution.Projects.First().ServiceName}

  deploy-environment-dev:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: dev
      profile: dev02
      workspace: dev-{solution.Projects.First().ServiceName}

  deploy-environment-prod:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: prod
      profile: prod02
      workspace: prod-{solution.Projects.First().ServiceName}
";
}