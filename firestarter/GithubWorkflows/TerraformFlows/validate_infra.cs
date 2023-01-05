namespace firestarter.GithubWorkflows.TerraformFlows;

public static class validate_infra
{
    public static string content = @"
name: Validate infra

on:
  pull_request:
    branches:
      - main
  workflow_dispatch:

jobs:
  validate-environment-dev:
    secrets: inherit
    uses: ./.github/workflows/validate-common.yml
    with:
      environment: dev
      profile: dev02
      workspace: dev-tbd-integration-api

  validate-environment-stage:
    secrets: inherit
    uses: ./.github/workflows/validate-common.yml
    with:
      environment: stage
      profile: stage02
      workspace: stage-tbd-integration-api

  validate-environment-prod:
    secrets: inherit
    uses: ./.github/workflows/validate-common.yml
    with:
      environment: prod
      profile: prod02
      workspace: prod-tbd-integration-api
";

}