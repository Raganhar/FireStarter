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
      - name: transition-jira-issues
        uses: Raganhar/nup-github-action-jira-transition@v2
        env:
          GITHUB_CONTEXT: ""${{{{ toJson(github) }}}}""
        with:
          jira-api-key: ${{{{ secrets.JIRA_API_TOKEN }}}}
          jira-url: ${{{{ secrets.JIRA_BASE_URL }}}}
          jira-user: ${{{{ secrets.JIRA_USER_EMAIL }}}}
          branch_to_compare_to: main
          main-jira-transition: pending release
          release-jira-transition: QA
          jira_state_when_revert: blocked
          ignore_tickets_in_following_states: pending release
    
  transition-jira-issues-for-main:
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3
      - name: transition-jira-issues
        uses: Raganhar/nup-github-action-jira-transition@v2
        env:
          GITHUB_CONTEXT: ""${{{{ toJson(github) }}}}""
        with:
          jira-api-key: ${{{{ secrets.JIRA_API_TOKEN }}}}
          jira-url: ${{{{ secrets.JIRA_BASE_URL }}}}
          jira-user: ${{{{ secrets.JIRA_USER_EMAIL }}}}
          branch_to_compare_to: main
          main-jira-transition: pending release
          release-jira-transition: QA
          jira_state_when_revert: blocked
";
}