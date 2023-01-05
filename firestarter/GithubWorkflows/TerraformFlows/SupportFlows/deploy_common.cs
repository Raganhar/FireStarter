namespace firestarter.GithubWorkflows.TerraformFlows;

public static class deploy_common
{
    public static string content = @"name: deploy infra reuse

on:
  workflow_call:
    inputs:
      environment:
        required: true
        type: string
        description: ""The github specific environment""
      profile:
        required: true
        type: string
        description: ""The aws cli profile""
      workspace:
        required: true
        type: string
        description: ""The environment workspace""

concurrency:
  group: terraform-automation-${{ inputs.environment }}
  cancel-in-progress: false

permissions:
  id-token: write
  contents: read
  pull-requests: write


defaults:
  run:
    shell: bash
    working-directory: terraform

jobs:
  terraform_plan:
    name: 'Terraform Plan'
    runs-on: ubuntu-latest
    environment: ${{ inputs.environment }}
    env:
      TF_ROOT: .

    steps:
      - name: Checkout
        uses: actions/checkout@v3
        with:
          ref: ${{ env.GITHUB_REF }}

      - name: Change file permissions
        run: |
          chmod +x *.sh

      # Sign into Org account, containing the terraform state files
      - name: Sign into Org account
        uses: ""./.github/workflows/sign-in-to-org-account""

      # Sign into dev account
      - name: Sign into environment account
        uses: ""./.github/workflows/sign-in-to-environment-account""
        with:
          environment: ${{ inputs.environment }}
          profile: ${{ inputs.profile }}
          account-id: ${{ secrets.ACCOUNT_ID }}

      - name: HashiCorp - Setup Terraform
        uses: hashicorp/setup-terraform@v2.0.0
        with:
          terraform_version: 1.3.0

      # Generates an execution plan for Terraform
      - name: Terraform Init
        id: init
        run:
          terraform init -backend-config=""access_key=${{ env.TF_STATE_AWS_ACCESS_KEY_ID }}"" -backend-config=""secret_key=${{ env.TF_STATE_AWS_SECRET_ACCESS_KEY }}"" -backend-config=""token=${{ env.TF_STATE_AWS_SESSION_TOKEN }}""

      - name: Terraform Plan
        id: plan
        run: |
          terraform workspace select ${{ inputs.workspace }}
          terraform plan -no-color $(./terraformGenerateArguments.sh -e ${{ inputs.environment }}) -out tf_plan -lock-timeout=300s 2> /dev/null

      - name: Upload TF Plan
        uses: actions/upload-artifact@v2
        with:
          name: tf_plan
          path: ./terraform/tf_plan
          if-no-files-found: error
          retention-days: 1

  terraform_apply:
    name: 'Terraform Apply'
    runs-on: ubuntu-latest
    needs: terraform_plan
    environment: ${{ inputs.environment }}

    steps:
      - name: Checkout
        uses: actions/checkout@v3
        with:
          ref: ${{ env.GITHUB_REF }}

      - name: Change file permissions
        run: |
          chmod +x *.sh

      # Sign into Org account, containing the terraform state files
      - name: Sign into Org account
        uses: ""./.github/workflows/sign-in-to-org-account""

      # Sign into dev account
      - name: Sign into environment account
        uses: ""./.github/workflows/sign-in-to-environment-account""
        with:
          environment: ${{ inputs.environment }}
          profile: ${{ inputs.profile }}
          account-id: ${{ secrets.ACCOUNT_ID }}

      - name: HashiCorp - Setup Terraform
        uses: hashicorp/setup-terraform@v2.0.0
        with:
          terraform_version: 1.3.0

      - name: Terraform Init
        id: init
        run:
          terraform init -backend-config=""access_key=${{ env.TF_STATE_AWS_ACCESS_KEY_ID }}"" -backend-config=""secret_key=${{ env.TF_STATE_AWS_SECRET_ACCESS_KEY }}"" -backend-config=""token=${{ env.TF_STATE_AWS_SESSION_TOKEN }}""

      - name: Download TF Plan
        uses: actions/download-artifact@v2
        with:
          name: tf_plan
          path: ./terraform/tf_plan

      - name: Terraform Apply
        id: apply
        run: |
          terraform workspace select ${{ inputs.workspace }}
          terraform show ""tf_plan/tf_plan"" -no-color
          terraform apply -lock-timeout=300s -auto-approve ""tf_plan/tf_plan"" 
";
}