$gitRepos = @("auto-generate-pipeline-files")
foreach ($repo in $gitRepos) {
    git clone  ("https://github.com/Raganhar/" +$repo+".git") 
    cd auto-generate-pipeline-files
    # git checkout -b update-pipeline
    dotnet run --project C:\Code\FireStarter\firestarter\
    git add -A
    git commit -a -m 'ran firestarter'
    # git push -u origin update-pipeline
    git push origin 
    cd..
    # rm -R $repo
}

