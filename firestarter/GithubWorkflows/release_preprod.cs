﻿namespace firestarter.GithubWorkflows;

public static class release_preprod
{
    public static string content(List<Project> projects) => $@"name: Release preprod [preprod]    

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

  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projects.Select(x=>($@"release-{x.Name}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: preprod
      prefix: preprod
      cluster: autoproff-cluster
      service_name: {x.Name}-service
      dockerfile: ""{x.Name}-dockerfile""
      {(string.IsNullOrWhiteSpace(x.ContainerName) ? "container_name: stage-{x.ContainerName}" : "")}"
    )))}
";
}