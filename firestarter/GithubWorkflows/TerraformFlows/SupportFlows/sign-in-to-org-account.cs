namespace firestarter.GithubWorkflows.TerraformFlows.SupportFlows;

public static class sign_in_to_org_account
{
    public static string content = @"name: sign_in_env_account

runs:
  using: ""composite""
  steps:
    - name: Configure AWS credentials from Organization account
      uses: aws-actions/configure-aws-credentials@v1
      with:
        role-to-assume: arn:aws:iam::284583527817:role/github
        aws-region: eu-central-1

    - name: Save temporary credentials for terraform init
      shell: bash
      run: |
        echo ""TF_STATE_AWS_SESSION_TOKEN=$AWS_SESSION_TOKEN"" >> $GITHUB_ENV
        echo ""TF_STATE_AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID"" >> $GITHUB_ENV
        echo ""TF_STATE_AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY"" >> $GITHUB_ENV
";
}