namespace firestarter.GithubWorkflows;

public static class release_preprod
{
    public static string content(List<Project> projects) => $@"name: Release preprod [DEV03]    

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
            ref: 'refs/tags/${{{{ steps.date.outputs.date }}}}',
            sha: context.sha
          }})

  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projects.Select(x=>($@"release-{x.ServiceName}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: dev03
      prefix: dev03
      cluster: autoproff-cluster
      service_name: {x.ServiceName}
      dockerfile: ""{x.DockerFile}""
      branch_name: main
      {(!string.IsNullOrWhiteSpace(x.LegacyProperties?.ContainerName) ? $"container_name: preprod-{x.LegacyProperties.ContainerName}" : "")}"
    )))}
";
}