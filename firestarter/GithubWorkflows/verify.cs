namespace firestarter.GithubWorkflows;

public static class verify
{
  public static string content(List<Project> projects) => $@"name: verify

on:
  pull_request:
  push:
    branches:
      - master
      - main
      - dev
      - release
      
jobs:
  verify:
    runs-on: ubuntu-latest
    steps:
      - name: ""Checkout""
        uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '{(projects.GroupBy(c => c.Tech).Count() == 1 && projects.GroupBy(c => c.Tech).First().Key == TechStack.legacy_dotnet ? "3" : "6")}.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --no-restore {(!string.IsNullOrWhiteSpace(projects.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.TestFilter))?.TestFilter) ? $"--filter {projects.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.TestFilter))?.TestFilter}" : " --filter Category!=SanityTest")}

  {string.Join(Environment.NewLine+Environment.NewLine+"  ",projects.Select(x=>$@"build_n_push_docker_image_{x.Name}:
    needs: verify
    runs-on: ubuntu-latest
    steps:
    - name: Dump GitHub context
      env:
        GITHUB_CONTEXT: ${{{{ toJson(github) }}}}
      run: |
        echo ""$GITHUB_CONTEXT""

    - name: ""Get branch name and save to env""
      env:
        IS_PR: ${{{{ github.event_name == 'pull_request'}}}}
      run: |
        
        if ${{IS_PR}}; then
          BRANCH=""${{GITHUB_HEAD_REF}}""
          BASE_TAG=""${{BRANCH##*/}}""
        else
          BRANCH=""${{GITHUB_REF_NAME}}""
          BASE_TAG=""${{BRANCH##*/}}""
        fi

        echo ""BRANCH=${{BRANCH}}"" >> $GITHUB_ENV 
        echo ""BASE_TAG=${{BASE_TAG}}"" >> $GITHUB_ENV 
    - name: Checkout
      uses: actions/checkout@v3
      with:
          token: ${{{{ secrets.MY_PAT }}}}
          fetch-depth: 0

    - name: Set Tag Name
      id: artifact_version
      run: echo ""artifact_version=${{{{env.BASE_TAG}}}}.$(date +'%Y-%m-%d').b${{{{github.run_number}}}}"" >> $GITHUB_ENV 

    - name: artifact version
      run: echo newly set env variable ${{{{env.BASE_TAG}}}}

    - name: container tag
      run: echo newly set env variable ${{{{env.artifact_version}}}}

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v1

    - id: ""lower_repo""
      uses: ASzc/change-string-case-action@v2
      with:
        string: ${{{{ github.event.repository.name }}}}

    - id: ""lower_owner""
      uses: ASzc/change-string-case-action@v2
      with:
        string: ${{{{ github.repository_owner }}}}

    - name: Authenticate with the Github Container Registry 🔐
      run: echo ${{{{ secrets.GITHUB_TOKEN }}}} | docker login ghcr.io -u USERNAME --password-stdin

    - name: Build and push docker image 🏗 📦
      id: build-n-push
      uses: docker/build-push-action@v2
      with:
        context: .
        file: {x.DockerFile}
        platforms: linux/amd64
        push: true
        tags: ghcr.io/${{{{steps.lower_owner.outputs.lowercase}}}}/{x.ServiceName}:${{{{env.artifact_version}}}}

    - name: Create tag
      if: steps.build-n-push.outcome == 'success' 
      run: |
        git tag ${{{{ env.artifact_version }}}}
        git push --tags"))}
";
}