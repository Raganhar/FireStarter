namespace firestarter.GithubWorkflows;

public static class verify
{
    public static string content = @"name: Promote dev

on:
  workflow_dispatch:
  schedule:
    - cron: '0 0 * * 1,2,3,4,5' # UTC time

jobs:
  create-branch:
    name: ""Create branch release from current dev""
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v3
        with:
          ref: dev

      - name: Remove old release branch
        continue-on-error: true # Fails if there is no old release branch
        run: |
          git fetch --all
          git push origin --delete release

      - name: Create new release branch
        id: create-branch
        run: |
          git config user.name github-actions
          git config user.email github-actions@autoproff.com
          git checkout -b ""release""
          git push --set-upstream origin ""release""

      - name: transition jira tickets
        uses: Raganhar/nup-github-action-jira-transition@v2
        env:
          GITHUB_CONTEXT: ""${{{{ toJson(github) }}}}""
        with:
          jira-api-key: ${{{{ secrets.JIRA_API_TOKEN }}}}
          jira-url: ${{{{ secrets.JIRA_BASE_URL }}}}
          jira-user: ${{{{ secrets.JIRA_USER_EMAIL }}}}
          branch_to_compare_to: main
          main-jira-transition: done
          release-jira-transition: QA
          jira_state_when_revert: blocked
          ignore_tickets_in_following_states: pending release
";
}