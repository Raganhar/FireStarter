namespace firestarter.LegacyFlows;

public static class apfe_release_preprod
{
    public static string content(List<string> projectNames) => $@"name: Release preprod [DEV03]    

on:
  push:
    branches:
      - main
jobs:
  create-tag:
    runs-on: ubuntu-latest
    steps:
    - name: Set Tag Name
      id: date
      run: echo ""::set-output name=date::$(date +'%Y-%m-%d')""
    - name: Create tag
      uses: actions/github-script@v5
      with:
        script: |
          github.rest.git.createRef({{
            owner: context.repo.owner,
            repo: context.repo.repo,
            ref: 'refs/tags/${{{{ steps.date.outputs.date }}}}',
            sha: context.sha
          }})

  release-api:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev03
      vue_buildmode: production
      bucket_name: dev03-autoproff-auction-application
      branch_name: main
";
}