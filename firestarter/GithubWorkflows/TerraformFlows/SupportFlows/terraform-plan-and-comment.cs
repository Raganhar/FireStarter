namespace firestarter.GithubWorkflows.TerraformFlows.SupportFlows;

public static class terraform_plan_and_comment
{
    public static string content = @"name: terraform-plan-and-comment

inputs:
  environment:
    required: true
    description: text to display
    type: string
  workspace:
    required: true
    description: actual workspace to work on
    type: string
  github-token:
    required: true
    description: github token to comment on the PR

outputs:
  result:
    description: ""output of terraform plan""
    value: ${{ steps.plan.outcome }}

runs:
  using: ""composite""
  steps:
    - name: Terraform Plan ${{ inputs.environment }}
      id: plan
      shell: bash
      run: |
        cd terraform
        terraform workspace select ${{ inputs.workspace }}
        terraform plan -no-color $(./terraformGenerateArguments.sh -e ${{ inputs.environment }}) -out ${{ inputs.environment }}-tfplan.binary -lock-timeout=300s 2> /dev/null
        terraform show ${{ inputs.environment }}-tfplan.binary -no-color
      continue-on-error: true

    - name: Echo ${{ inputs.environment }} plan output to a file
      shell: bash
      run: |
        echo -e '${{ steps.plan.outputs.stdout }}' > ${{ inputs.environment }}.out
        echo -e '${{ steps.plan.outputs.stderr }}' >> ${{ inputs.environment }}.out

    - uses: actions/github-script@v6
      name: Terraform ${{ inputs.environment }} PR commenter
      with:
        github-token: ${{ inputs.github-token }}
        script: |
          // 1. Retrieve existing bot comments for the PR
          const { data: comments } = await github.rest.issues.listComments({
            owner: context.repo.owner,
            repo: context.repo.repo,
            issue_number: context.issue.number,
          })
          const botComment = comments.find(comment => {
            return comment.user.type === 'Bot' && comment.body.includes('Terraform ${{ inputs.environment }} Plan')
          })

          const { readFile } = require(""fs/promises"")
          const data = await readFile('${{ inputs.environment }}.out')


          // 2. Prepare format of the comment
          const output = `#### Terraform ${{ inputs.environment }} Plan 📖\`${{ steps.plan.outcome }}\`

          <details><summary>Show Plan</summary>

          \`\`\`\n
          ${data}
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

    # - name: Run infracost for ${{ inputs.environment }}
    #   continue-on-error: true
    #   shell: bash
    #   run: |
    #     terraform show -json ${{ inputs.environment }}-tfplan.binary > ${{ inputs.environment }}.json
    #     infracost breakdown --path ${{ inputs.environment }}.json --project-name ${{ inputs.workspace }} --format json --out-file ${{ inputs.environment }}-infracost.json
";
}