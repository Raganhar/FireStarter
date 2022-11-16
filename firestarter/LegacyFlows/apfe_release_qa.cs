namespace firestarter.LegacyFlows;

public static class apfe_release_qa
{
    public static string content(List<string> projectNames) => $@"name: Release QA [STAGE02]      

on:
  workflow_run:
    workflows:
      - Promote dev
    types:
      - completed
  push:
    branches:
      - ""release""
jobs:
  release-api:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: stage02
      vue_buildmode: production
      bucket_name: stage-autoproff-auction-application
      branch_name: release
";
}