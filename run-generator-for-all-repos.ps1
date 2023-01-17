$PAT = $args[0]
$gitRepos = @("AuctionService","ProductService","product-api")
foreach ($repo in $gitRepos) {

    git clone ("https://" + $PAT + "@github.com/AUTOProff/" + $repo + ".git") --depth 1
    cd $repo
    try {
        dotnet run --project C:\Code\FireStarter\firestarter\
        Write-Output "lol"    
        git add -A
        git commit -a -m 'ran firestarter'
        git push origin 
    }
    catch {
        Write-Output "test output"
    }

    cd..
    # rm -R $repo
}

