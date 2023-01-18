namespace firestarter.GithubWorkflows.TerraformFlows;

public static class AutoDeployInfra
{
    public static string content = @"
name: Deploy Infrastructure

on:
  push:
    branches:
      - main
  workflow_dispatch:

jobs:
  wait-for-verify:
    uses: lewagon/wait-on-check-action@v1.2.0
    with:
      ref: main
      check-name: verify
      repo-token: ${{ secrets.GITHUB_TOKEN }}
      wait-interval: 20

  deploy-environment-stage:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: stage
      profile: stage02
      workspace: stage-tbd-integration-api

  deploy-environment-dev:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: dev
      profile: dev02
      workspace: dev-tbd-integration-api

  deploy-environment-preprod:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: dev03
      profile: dev03
      workspace: dev03-tbd-integration-api

  deploy-environment-prod:
    needs: [wait-for-verify]
    secrets: inherit
    uses: ./.github/workflows/deploy-common.yml
    with:
      environment: prod
      profile: prod02
      workspace: prod-tbd-integration-api
";
}