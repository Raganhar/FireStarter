namespace firestarter.GithubWorkflows;

public static class release_prod
{
    public static string content(List<string> projectNames) => $@"name: Release prod [PROD02]

on:
  workflow_dispatch:

jobs:
  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projectNames.Select(x=>($@"release-{x}:
    secrets: inherit
    uses: ./.github/workflows/release-reuse.yml
    with:
      environment: prod02
      prefix: prod
      cluster: autoproff-cluster
      service_name: {x}-service
      dockerfile: ""{x}-dockerfile""
      container_name: prod-{x}-container")))}

  transition-jira-issues-on-trigger:
    runs-on: ubuntu-latest
    steps:
      - name: transition jira tickets
        uses: Raganhar/nup-github-action-jira-transition@v2
        env:
          GITHUB_CONTEXT: ""${{{{ toJson(github) }}}}""
        with:
          jira-api-key: ${{{{ secrets.JIRA_API_TOKEN }}}}
          jira-url: ${{{{ secrets.JIRA_BASE_URL }}}}
          jira-user: ${{{{ secrets.JIRA_USER_EMAIL }}}}
          main-jira-transition: done
          release-jira-transition: in progress
          branch_to_compare_to: main
          jira_state_when_revert: blocked
";
}