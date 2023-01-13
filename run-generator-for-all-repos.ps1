$gitRepos = @("AuctionService","ProductService","tbd-integration-api")
foreach ($repo in $gitRepos) {
    git clone  ("https://github.com/AUTOProff/" +$repo+".git") 
    cd $repo
    # git checkout -b update-pipeline
    dotnet run --project C:\Code\FireStarter\firestarter\
    git add -A
    git commit -a -m 'ran firestarter'
    # git push -u origin update-pipeline
    git push origin 
    cd..
    # rm -R $repo
}

