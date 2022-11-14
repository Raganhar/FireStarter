namespace firestarter.GithubWorkflows;

public static class pull_request_for_hotfix
{
    public static string content = @"name: Auto PR for main and release

env:
  TARGET_BRANCH: dev

on:
  pull_request:
    types:
      - closed
    branches:
      - main
      - release

jobs:
  if_merged:
    if: ${{{{ github.event.pull_request.merged == true && startsWith('hotfix', github.event.pull_request.headRefName) }}}}
    runs-on: ubuntu-latest
    steps:
    - name: ""Checkout""
      uses: actions/checkout@v3

    - name: Create pull request
      id: create-pull-request
      env:
        GITHUB_TOKEN: ${{{{ secrets.GITHUB_TOKEN }}}}
        CURRENT_BRANCH: ${{{{ github.event.pull_request.head.ref }}}}
      run: |
        gh pr create -B $TARGET_BRANCH -H $CURRENT_BRANCH --title ""Merge $CURRENT_BRANCH into $TARGET_BRANCH"" --body ""**Created by Github actions**""
";
}