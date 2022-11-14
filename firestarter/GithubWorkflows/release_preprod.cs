namespace firestarter.GithubWorkflows;

public static class release_preprod
{
    public static string content(List<string> projectNames) => $@"name: Release preprod [preprod]    

on:
  push:
    branches:
      - main

  workflow_dispatch:

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
            ref: 'refs/tags/${{ steps.date.outputs.date }}',
            sha: context.sha
          }})

  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projectNames.Select(x=>($@"release-{x}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: preprod
      prefix: preprod
      cluster: {x}-cluster
      service_name: {x}-service
      dockerfile: ""{x}-dockerfile""
      container_name: preprod-{x}-container")))}
";
}