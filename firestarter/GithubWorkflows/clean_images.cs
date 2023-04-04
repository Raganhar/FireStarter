namespace firestarter.GithubWorkflows;

public static class clean_images
{
  public static string content(SolutionDescription solution) => $@"
name: Clean images

on: 
  workflow_dispatch:
  schedule:
    - cron: ""0 0 * * *""  # every day at midnight

jobs:
  clean-ghcr:
      name: Delete old unused container images
      runs-on: ubuntu-latest
      steps:
        - name: delete containers
          uses: snok/container-retention-policy@v1.5.1
          with:
            image-names: {(string.Join(",",solution.Projects.Select(x=>x.ServiceName.ToLowerInvariant())))}
            cut-off: 14 days ago UTC
            account-type: org
            org-name: AUTOProff
            keep-at-least: 3
            token: ${{{{ secrets.GIT_PAT }}}}
";
}