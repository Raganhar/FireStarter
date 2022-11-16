namespace firestarter.LegacyFlows;

public static class apfe_release_dev
{
    public static string content (List<string> projectNames) => $@"name: Release dev [DEV02]

on:
  push:
    branches:
      - dev
jobs:
  release-frontend:
    name: Checkout
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev02
      vue_buildmode: development
      bucket_name: dev-autoproff-auction-application
      branch_name: dev
";
}