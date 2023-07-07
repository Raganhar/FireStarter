namespace firestarter.GithubWorkflows;

public static class transition_jira_issues
{
    public static string content = @"name: transition-jira-issues

on:
  pull_request_target:
      types:
      - closed
      branches:
      - main
      # - release
  push:
      branches:
      - release  

jobs:
  transition-jira-issues-for-release:
    if: github.ref == 'refs/heads/release'
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

    
  transition-jira-issues-for-main:
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

";
}