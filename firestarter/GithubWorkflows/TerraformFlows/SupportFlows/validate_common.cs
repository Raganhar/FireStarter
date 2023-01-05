namespace firestarter.GithubWorkflows.TerraformFlows;

public static class validate_common
{
    public static string content = @"name: validate infra reuse

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
        description: ""The terraform workspace""

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
      
      - name: Setup Infracost
        uses: infracost/actions/setup@v2
        with:
          api-key: ${{ secrets.INFRACOST_API_KEY }}

      # Sign into Org account, containing the terraform state files
      - name: Sign into Org account
        uses: ""./.github/workflows/sign-in-to-org-account""

      # Sign into account
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

      - name: Terraform Format
        id: fmt
        run: terraform fmt -check -recursive

      # Generates an execution plan for Terraform
      - name: Terraform Init
        id: init
        run:
          terraform init -backend-config=""access_key=${{ env.TF_STATE_AWS_ACCESS_KEY_ID }}"" -backend-config=""secret_key=${{ env.TF_STATE_AWS_SECRET_ACCESS_KEY }}"" -backend-config=""token=${{ env.TF_STATE_AWS_SESSION_TOKEN }}""

      - name: Terraform Validate
        id: validate
        run: terraform validate -no-color

      - uses: actions/github-script@v6
        name: Terraform validate PR comment
        if: github.event_name == 'pull_request'
        with:
          github-token: ${{ secrets.GITHUB_TOKEN }}
          script: |
            // 1. Retrieve existing bot comments for the PR
            const { data: comments } = await github.rest.issues.listComments({
              owner: context.repo.owner,
              repo: context.repo.repo,
              issue_number: context.issue.number,
            })
            const botComment = comments.find(comment => {
              return comment.user.type === 'Bot' && comment.body.includes('Terraform Initialization')
            })
            
            
            // 2. Prepare format of the comment
            const output = `#### Terraform Format and Style 🖌\`${{ steps.fmt.outcome }}\`
            #### Terraform Initialization ⚙️\`${{ steps.init.outcome }}\`
            #### Terraform Validation 🤖\`${{ steps.validate.outcome }}\`
            <details><summary>Validation Output</summary>
            
            \`\`\`\n
            ${{ steps.validate.outputs.stdout }}
            \`\`\`
            
            </details>
            
            *Pusher: @${{ github.actor }}, Action: \`${{ github.event_name }}\`, Working Directory: \`${{ env.tf_actions_working_dir }}\`, Workflow: \`${{ github.workflow }}\`*`;
            
            // 3. If we have a comment, update it, otherwise create a new one
            if (botComment) {
              github.rest.issues.updateComment({
                owner: context.repo.owner,
                repo: context.repo.repo,
                comment_id: botComment.id,
                body: output
              })
            } else {
              github.rest.issues.createComment({
                issue_number: context.issue.number,
                owner: context.repo.owner,
                repo: context.repo.repo,
                body: output
              })
            }

      
      - name: Terraform plan and PR comment for ${{ inputs.workspace }}
        uses: ""./.github/workflows/terraform-plan-and-comment""
        id: plan
        with:
          environment: ${{ inputs.environment }}
          workspace: ${{ inputs.workspace }}
          github-token: ${{ secrets.GITHUB_TOKEN }}
      

      - name: Terraform Plan Status
        if: steps.plan.outputs.result == 'failure'
        run: |
          exit 1

      # if the commenter only show All project cost, it is because there is no change in the project
      # follow this issue https://github.com/infracost/infracost/issues/1538
      - name: Infrascost commenter
        continue-on-error: true
        run: |
          infracost comment github --path ""*-infracost.json"" --repo=$GITHUB_REPOSITORY --pull-request=${{github.event.pull_request.number}} --github-token=${{github.token}} --behavior update

      - name: Run tfsec commenter
        uses: aquasecurity/tfsec-pr-commenter-action@v1.2.0
        with:
          github_token: ${{ github.token }}
";
}