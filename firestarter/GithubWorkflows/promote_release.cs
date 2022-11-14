namespace firestarter.GithubWorkflows;

public static class promote_release
{
    public static string content = @"name: Promote release

env:
  TARGET_BRANCH: main
  RELEASE_BRANCH: release

on:
  workflow_dispatch:
  schedule:
    - cron: '30 12 * * 2,4'
    
jobs:
  create-branch:
    name: ""Create PR for Main""
    runs-on: ubuntu-latest
    steps:
      - name: ""Checkout""
        uses: actions/checkout@v3
        with:
          ref:
            ${{{{ env.RELEASE_BRANCH }}}}

      - name: Create pull request
        id: create-pull-request
        continue-on-error: true
        env:
          GITHUB_TOKEN: ${{{{ secrets.GITHUB_TOKEN }}}}
          TARGET_BRANCH: ${{{{ env.TARGET_BRANCH }}}}
          RELEASE_BRANCH: ${{{{ env.RELEASE_BRANCH }}}}
        run: |
          echo $RELEASE_BRANCH
          echo $TARGET_BRANCH 
          gh pr create -B $TARGET_BRANCH -H $RELEASE_BRANCH --title ""Merge $RELEASE_BRANCH into $TARGET_BRANCH"" --body ""Created by Github action, complete to start release to preprod 🚀""
      
      - name: No code to release
        if: steps.create-pull-request.outcome == 'failure'
        run : echo '🌞 No new code to release 🌞'

";
}