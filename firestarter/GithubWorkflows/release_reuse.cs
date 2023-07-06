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
      branch_name:
        required: true
        type: string
    secrets:
      AWS_ACCESS_KEY_ID:
        required: true
      AWS_SECRET_ACCESS_KEY:
        required: true

jobs:    
  deploy:
    name: ""Deploy""
    runs-on: ubuntu-latest
    environment: ${{ inputs.environment }}
    steps:
      - name: Checkout
        uses: actions/checkout@v3
        with:
          ref: ${{ inputs.branch_name }}

      - name: ""Find tag to release""
        env:
          IS_TAG_CONTEXT: ${{ github.ref_type == 'tag' }}
        run: |
          if ${IS_TAG_CONTEXT}; then
            TAG=""${{github.ref_name}}""
          else
            git fetch --prune --unshallow
            TAG=$(git describe --tag --abbrev=0)
          fi
          echo ""TAG=${TAG}"" >> $GITHUB_ENV

      - name: container tag
        run: echo newly set env variable ${{env.TAG}}

      - id: ""lower_repo""
        run: |
            repo_lower=$(echo ""${{{{  github.event.repository.name }}}}"" | awk '{{print tolower($0)}}' )
            echo ""lowercase=$repo_lower"" >> ""$GITHUB_OUTPUT""
      - id: ""lower_owner""
        run: |
          owner_lower=$(echo ""${{{{ github.repository_owner }}}}"" | awk '{{print tolower($0)}}')
          echo ""lowercase=$owner_lower"" >> ""$GITHUB_OUTPUT""

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v1
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ inputs.region }}

      - name: Login to Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v1

      - name: Log in to the Container registry
        uses: docker/login-action@v1
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

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

      - name: Pull Image from ghcr.io
        id: build-image
        env:
          ECR_REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          ECR_REPOSITORY: ${{ env.CONTAINER_IMAGE_NAME }}
        run: |
          echo ""pulling image from ghcr""
          docker pull ghcr.io/${{steps.lower_owner.outputs.lowercase}}/${{inputs.service_name}}:${{env.TAG}}
          
          echo ""renaming/tagging image from to be the same as ECR image""
          docker tag ghcr.io/${{steps.lower_owner.outputs.lowercase}}/${{inputs.service_name}}:${{env.TAG}} $ECR_REGISTRY/$ECR_REPOSITORY:latest
          
          echo ""pushing image to ecr""
          docker push $ECR_REGISTRY/$ECR_REPOSITORY:latest

          echo ""image=$ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG"" >> $GITHUB_OUTPUT

      - name: Force ECS update
        env:
          ECS_SERVICE: ${{ inputs.prefix }}-${{ inputs.service_name }}
          ECS_CLUSTER: ${{ inputs.prefix }}-${{ inputs.cluster }}
        run: |
          aws ecs update-service --cluster $ECS_CLUSTER --service $ECS_SERVICE --force-new-deployment
";
}