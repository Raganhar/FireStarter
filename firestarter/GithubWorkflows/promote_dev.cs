namespace firestarter.GithubWorkflows;

public static class promote_dev
{
    public static string content = @"name: Promote dev

on:
  workflow_dispatch:
  schedule:
    - cron: '30 10 * * 1,3' # UTC time

jobs:
  create-branch:
    name: ""Create branch release from current dev""
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v3
        with:
          ref: dev
          token: ${{ secrets.MY_PAT }}

      - name: Remove old release branch
        continue-on-error: true # Fails if there is no old release branch
        run: |
          git fetch --all

      - name: Create new release branch
        id: create-branch
        run: |
          git config user.name github-actions
          git config user.email github-actions@autoproff.com
          git checkout -b ""release""
          git reset --hard dev
          git push --force origin release

      - name: transition jira tickets
        uses: AUTOProff/ap-github-action-jira-transition@v1
        env:
          GITHUB_CONTEXT: ""${{ toJson(github) }}""
        with:
          jira-api-key: ${{ secrets.JIRA_API_TOKEN }}
          jira-url: ${{ secrets.JIRA_BASE_URL }}
          jira-user: ${{ secrets.JIRA_USER_EMAIL }}
          branch_to_compare_to: main
          main-jira-transition: done
          release-jira-transition: QA
          jira_state_when_revert: blocked
          ignore_tickets_in_following_states: pending release

      - name: Checkout release
        uses: actions/checkout@v3
        with:
            token: ${{ secrets.MY_PAT }}
            ref: release
            fetch-depth: 0

      - name: Create release tag
        run: |
          git tag ${{ env.artifact_version }}
          git push --tags
";
}