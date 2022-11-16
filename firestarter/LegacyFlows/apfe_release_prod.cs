namespace firestarter.LegacyFlows;

public static class apfe_release_prod
{
    public static string content(List<string> projectNames) => $@"name: Release prod [PROD02]

on:
  workflow_dispatch:

jobs:
  release-api:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: prod02
      vue_buildmode: production
      bucket_name: prod-autoproff-auction-application
      branch_name: main
";
}