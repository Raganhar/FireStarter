namespace firestarter.GithubWorkflows.TerraformFlows.SupportFlows;

public static class sign_in_to_environment_account
{
    public static string content = @"name: sign_in_account

inputs:
  environment:
    required: true
  account-id:
    required: true
  profile:
    required: true

runs:
  using: ""composite""
  steps:
    - name: Configure AWS credentials from ${{ inputs.environment }} account
      uses: aws-actions/configure-aws-credentials@v1
      with:
        role-to-assume: arn:aws:iam::${{ inputs.account-id }}:role/github
        aws-region: eu-central-1

    - name: Add profile credentials to ~/.aws/credentials
      shell: bash
      run: |
        aws configure set aws_access_key_id $AWS_ACCESS_KEY_ID --profile autoproff-${{ inputs.profile }}
        aws configure set aws_secret_access_key $AWS_SECRET_ACCESS_KEY --profile autoproff-${{ inputs.profile }}
        aws configure set aws_session_token $AWS_SESSION_TOKEN --profile autoproff-${{ inputs.profile }}";
}