$PAT=$args[0]
$gitRepos = @("AuctionService")
foreach ($repo in $gitRepos) {

    git clone ("https://"+$PAT+"@github.com/AUTOProff/" +$repo+".git") --depth 1
    cd $repo
    # git checkout -b update-pipeline
try{
    dotnet run --project C:\Code\FireStarter\firestarter\
    git add -A
    git commit -a -m 'ran firestarter'
    # git push -u origin update-pipeline
    git push origin 
} catch{}

    cd..
    # rm -R $repo
}

