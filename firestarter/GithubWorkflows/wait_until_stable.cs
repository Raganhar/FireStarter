namespace firestarter.GithubWorkflows;

public static class wait_until_stable
{
    public static string content = @"name: wait-until-stable

on:
  workflow_call:
    inputs:
      prefix:
        required: true
        type: string
        description: ""Will be used as a prefix for all environment specific aws elements""
      service_name:
        required: true
        type: string
      region:
        required: false
        type: string
        default: eu-central-1
      cluster:
        required: false
        type: string
        default: autoproff-cluser
      environment:
        required: true
        type: string
        description: ""The github specific environment""
    secrets:
      AWS_ACCESS_KEY_ID:
        required: true
      AWS_SECRET_ACCESS_KEY:
        required: true

jobs:    
  deploy:
    name: ""Wait until stable""
    runs-on: ubuntu-latest
    environment: ${{ inputs.environment }}
    steps:
      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v1
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ inputs.region }}

      - name: Login to Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v1

      - name: Wait until stable
        env:
          ECS_SERVICE: ${{ inputs.prefix }}-${{ inputs.service_name }}
          ECS_CLUSTER: ${{ inputs.prefix }}-${{ inputs.cluster }}
        run: |
          aws ecs wait services-stable --cluster $ECS_CLUSTER --service $ECS_SERVICE
";
}