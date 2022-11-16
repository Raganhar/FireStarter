namespace firestarter.LegacyFlows;

public static class apfe_release_reuse
{
    public static string content = @"name: release-frontend-application

on:
  workflow_call:
    inputs:
      region:
        required: false
        type: string
        default: eu-central-1
      environment:
        required: true
        type: string
        description: ""The github specific environment""
      vue_buildmode:
        required: true
        type: string
        description: ""Vue build mode""
        default: ""development""
      bucket_name:
        required: true
        type: string
        description: ""S3 destination bucket""
      branch_name:
        required: true
        type: string
    secrets:
      AWS_ACCESS_KEY_ID:
        required: true
      AWS_SECRET_ACCESS_KEY:
        required: true
      AWS_DISTRIBUTION_ID:
        required: true

jobs:    
  deploy:
    name: Build and deploy frontend
    runs-on: ubuntu-latest
    environment: ${{{{ inputs.environment }}}}
    steps:
      - name: Checkout branch
        uses: actions/checkout@v3
        with:
          ref: ${{{{ inputs.branch_name }}}}

      - name: Copy configuration
        id: config
        run: |
          cp config/${{{{ inputs.environment }}}}/.env .env

      # Very explicit configure new relic steps. Cannot be included from reuseable file
      - run: sed -i 's/NEW_RELIC_ACCOUNT_ID/${{{{ secrets.NEW_RELIC_ACCOUNT_ID }}}}/g' public/js/newrelic.js
        shell: bash
      - run: sed -i 's/NEW_RELIC_TRUST_KEY/${{{{ secrets.NEW_RELIC_TRUST_KEY }}}}/g' public/js/newrelic.js
        shell: bash
      - run: sed -i 's/NEW_RELIC_AGENT_ID/${{{{ secrets.NEW_RELIC_AGENT_ID }}}}/g' public/js/newrelic.js
        shell: bash
      - run: sed -i 's/NEW_RELIC_LICENSE_KEY/${{{{ secrets.NEW_RELIC_LICENSE_KEY }}}}/g' public/js/newrelic.js
        shell: bash
      - run: sed -i 's/NEW_RELIC_APPLICATION_ID/${{{{ secrets.NEW_RELIC_APPLICATION_ID }}}}/g' public/js/newrelic.js
        shell: bash

      - name: Use npm 
        uses: actions/setup-node@v3
        with:
          node-version: 16
        
      - name: Run npm install
        run: npm install
          
      - name: Run npm build
        run: npm run build --mode ${{{{ inputs.vue_buildmode }}}} 

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v1
        with:
          aws-access-key-id: ${{{{ secrets.AWS_ACCESS_KEY_ID }}}}
          aws-secret-access-key: ${{{{ secrets.AWS_SECRET_ACCESS_KEY }}}}
          aws-region: ${{{{ inputs.region }}}}

      - name: Upload to s3
        run: |
          aws s3 sync dist s3://${{{{ inputs.bucket_name}}}}

      - name: Invalidate cloudfront cache 
        run: | 
          aws cloudfront create-invalidation --distribution-id ${{{{ secrets.AWS_DISTRIBUTION_ID }}}} --paths ""/*""

";
}