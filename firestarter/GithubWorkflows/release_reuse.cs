namespace firestarter.GithubWorkflows;

public static class release_reuse
{
    public static string content = @"name: release-container

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
      container_name:
        required: false
        type: string
        description: ""Optional, if not provided [prefix]-[service_name] will be used instead""
      region:
        required: false
        type: string
        default: eu-central-1
      cluster:
        required: false
        type: string
        default: autoproff-cluser
      dockerfile:
        required: false
        type: string
        default: ""Dockerfile""
      environment:
        required: true
        type: string
        description: ""The github specific environment""
    secrets:
      AWS_ACCESS_KEY_ID:
        required: true
      AWS_SECRET_ACCESS_KEY:
        required: true
    branch_name:
        required: true
        type: string

jobs:    
  deploy:
    name: ""Deploy""
    runs-on: ubuntu-latest
    environment: ${{ inputs.environment }}
    steps:
      - name: Checkout
        uses: actions/checkout@v3

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v1
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ inputs.region }}

      - name: Login to Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v1

      - name: Set container_image_name
        env:
          CONTAINER_NAME: ${{ inputs.prefix }}-${{ inputs.service_name }}
        run: |
          echo CONTAINER_IMAGE_NAME=""$CONTAINER_NAME"" >> $GITHUB_ENV

      - name: Override container_image_name
        id: override-container-name
        if: inputs.container_name
        env:
          CONTAINER_NAME: ${{ inputs.container_name }}
        run: |
          echo CONTAINER_IMAGE_NAME=""$CONTAINER_NAME"" >> $GITHUB_ENV

      - name: Build, tag, and push image to Amazon ECR
        id: build-image
        env:
          ECR_REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          IMAGE_TAG: ${{ github.sha }}
          ECR_REPOSITORY: ${{ env.CONTAINER_IMAGE_NAME }}
          DOCKERFILE: ${{ inputs.dockerfile }}
        run: |
          # Build and tag image
          docker build -t $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG -f $DOCKERFILE .
          docker tag $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG $ECR_REGISTRY/$ECR_REPOSITORY:latest
          # Push image
          docker push $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG
          docker push $ECR_REGISTRY/$ECR_REPOSITORY:latest
          # echo ""::set-output name=image::$ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG""

      - name: Update ECS image version
        id: use-image
        env:
          ECS_SERVICE: ${{ inputs.prefix }}-${{ inputs.service_name }}
          ECS_CLUSTER: ${{ inputs.prefix }}-${{ inputs.cluster }}
        run: |
          aws ecs update-service --cluster $ECS_CLUSTER --service $ECS_SERVICE --force-new-deployment 
";
}