using System.Diagnostics;
using firestarter.GithubActionModels.Pullrequest;
using firestarter.GithubWorkflows;
using firestarter.LegacyFlows;
using MoreLinq;
using Newtonsoft.Json;

namespace firestarter;

public class Logic
{
    private readonly ILogger _logger;
    private readonly ActionInputs _options;
    private readonly GithubActionContext_pullrequest _githubContext;
    private string _currentBranchName;
    public static string _solutionDescriptionPath = ".ap/solution.json";


    public Logic(ILogger logger, ActionInputs options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task DoDaThing()
    {
        SetToRootFolder();
        var description = GetSolutionDescription();
        //create folders
        var githubWorkflows = GenerateFolders(description);
        //create files
        SetToRootFolder();
        CreateGithubFiles(description, githubWorkflows);

        //infra
        //projects
        SetToRootFolder();
        // PrintEverything();
        PushStuffToGit();
    }

    private static string GenerateFolders(SolutionDescription? description)
    {
        var projectFolders = description.Projects.Select(x => x.Name)
            .SelectMany(x => new List<string> { $"{x}", $"{x}.tests" });
        var githubWorkflows = ".github/workflows";
        // new[] { githubWorkflows, ".infra" }.Concat(projectFolders).ForEach(x => { Directory.CreateDirectory(x); });
        new[] { githubWorkflows}.ForEach(x => { Directory.CreateDirectory(x); });
        return githubWorkflows;
    }

    private void SetToRootFolder()
    {
        if (string.IsNullOrWhiteSpace(_options.Directory))
        {
            _options.Directory = Directory.GetCurrentDirectory();
        }
        else
        {
            Directory.SetCurrentDirectory(_options.Directory);
        }
    }

    private static void PushStuffToGit()
    {
        string gitCommand = "git";
        string gitAddArgument = @"add -A";
        string gitCommitArgument = @"commit -m ""Auto generated stuff""";
        string gitPushArgument = @"push origin autogenerated-pipeline-files";

        // Process.Start(gitCommand, gitAddArgument);
        // Process.Start(gitCommand, gitCommitArgument);
        // Process.Start(gitCommand, gitPushArgument);
    }

    private void PrintEverything()
    {
        foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*",
                     SearchOption.AllDirectories))
        {
            _logger.LogInformation(file);
            Console.WriteLine(file);
        }

        var bob = new List<string> { "", "" };

        var b = new[] { "", "" };

        var list = b.GroupBy(x=>x).ToDictionary(x=>x.Key,c=>c);
        
    }

    private static SolutionDescription? GetSolutionDescription()
    {
        if (File.Exists(_solutionDescriptionPath))
        {
            var readAllText = File.ReadAllText(_solutionDescriptionPath);
            return JsonConvert.DeserializeObject<SolutionDescription>(readAllText);
        }

        throw new ArgumentException($"unable to find solution description at {_solutionDescriptionPath}");
    }

    private void CreateGithubFiles(SolutionDescription solutionDescription, string githubWorkflows)
    {
        Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), githubWorkflows));

        var projectNames = solutionDescription.Projects.Select(x => x.Name).ToList();

        switch (solutionDescription.LegacySystem)
        {
            case SupportedLegacySystems.apfe:Apfe(projectNames);
                break;
            // case SupportedLegacySystems.auction_service:AuctionService(solutionDescription);
            //     break;
            case null:
                if (solutionDescription.GitWorkflow == GitWorkflow.TrunkBased)
                {
                    TrunkBasedGithubFlows(solutionDescription);
                }
                else
                {
                    StandardGithubFlows(solutionDescription);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void Apfe(List<string> projectNames)
    {
        var filename = $"{nameof(promote_dev)}.yml".ToFileName();
        File.WriteAllText(filename, promote_dev.content);

        filename = $"{nameof(promote_release)}.yml".ToFileName();
        File.WriteAllText(filename, promote_release.content);

        filename = $"{nameof(pull_request_for_hotfix)}.yml".ToFileName();
        File.WriteAllText(filename, pull_request_for_hotfix.content);

        filename = $"{nameof(release_dev)}.yml".ToFileName();
        File.WriteAllText(filename, apfe_release_dev.content(projectNames));

        filename = $"{nameof(release_preprod)}.yml".ToFileName();
        File.WriteAllText(filename, apfe_release_preprod.content(projectNames));

        filename = $"{nameof(release_prod)}.yml".ToFileName();
        File.WriteAllText(filename, apfe_release_prod.content(projectNames));

        filename = $"{nameof(release_qa)}.yml".ToFileName();
        File.WriteAllText(filename, apfe_release_qa.content(projectNames));

        filename = $"{nameof(release_reuse)}.yml".ToFileName();
        File.WriteAllText(filename, apfe_release_reuse.content);

        filename = $"{nameof(transition_jira_issues)}.yml".ToFileName();
        File.WriteAllText(filename, transition_jira_issues.content);
    }
    
    // private static void AuctionService(SolutionDescription solution)
    // {
    //     var projects = solution.Projects;
    //     var filename = $"{nameof(verify)}.yml".ToFileName();
    //     File.WriteAllText(filename, verify.content(projects));
    //     
    //     filename = $"{nameof(run_sanity_tests)}.yml".ToFileName();
    //     File.WriteAllText(filename, run_sanity_tests.content(projects));
    //     
    //     filename = $"{nameof(promote_dev)}.yml".ToFileName();
    //     File.WriteAllText(filename, promote_dev.content);
    //
    //     filename = $"{nameof(promote_release)}.yml".ToFileName();
    //     File.WriteAllText(filename, promote_release.content);
    //
    //     filename = $"{nameof(pull_request_for_hotfix)}.yml".ToFileName();
    //     File.WriteAllText(filename, pull_request_for_hotfix.content);
    //
    //     filename = $"{nameof(release_dev)}.yml".ToFileName();
    //     File.WriteAllText(filename, release_dev.content(solution));
    //
    //     filename = $"{nameof(release_preprod)}.yml".ToFileName();
    //     File.WriteAllText(filename, release_preprod.content(projects));
    //
    //     filename = $"{nameof(release_prod)}.yml".ToFileName();
    //     File.WriteAllText(filename, release_prod.content(solution));
    //
    //     filename = $"{nameof(release_qa)}.yml".ToFileName();
    //     File.WriteAllText(filename, release_qa.content(solution));
    //
    //     filename = $"{nameof(release_reuse)}.yml".ToFileName();
    //     File.WriteAllText(filename,   firestarter.LegacyFlows.AuctionService.release_reuse.content);
    //
    //     filename = $"{nameof(transition_jira_issues)}.yml".ToFileName();
    //     File.WriteAllText(filename, transition_jira_issues.content);
    // }

    private static void StandardGithubFlows(SolutionDescription solution)
    {
        var projects = solution.Projects;
        var filename = $"{nameof(verify)}.yml".ToFileName();
        File.WriteAllText(filename, verify.content(projects));
        
        filename = $"{nameof(clean_images)}.yml".ToFileName();
        File.WriteAllText(filename, clean_images.content(solution));
        
        filename = $"{nameof(run_sanity_tests)}.yml".ToFileName();
        File.WriteAllText(filename, run_sanity_tests.content(solution));
        
        filename = $"{nameof(promote_dev)}.yml".ToFileName();
        File.WriteAllText(filename, promote_dev.content);

        filename = $"{nameof(promote_release)}.yml".ToFileName();
        File.WriteAllText(filename, promote_release.content);

        filename = $"{nameof(pull_request_for_hotfix)}.yml".ToFileName();
        File.WriteAllText(filename, pull_request_for_hotfix.content);

        filename = $"{nameof(release_dev)}.yml".ToFileName();
        File.WriteAllText(filename, release_dev.content(solution));

        filename = $"{nameof(release_preprod)}.yml".ToFileName();
        File.WriteAllText(filename, release_preprod.content(projects));

        filename = $"{nameof(release_prod)}.yml".ToFileName();
        File.WriteAllText(filename, release_prod.content(solution));

        filename = $"{nameof(release_qa)}.yml".ToFileName();
        File.WriteAllText(filename, release_qa.content(solution));

        filename = $"{nameof(release_reuse)}.yml".ToFileName();
        File.WriteAllText(filename, release_reuse.content);

        filename = $"{nameof(transition_jira_issues)}.yml".ToFileName();
        File.WriteAllText(filename, transition_jira_issues.content);
    }
    private static void TrunkBasedGithubFlows(SolutionDescription solution)
    {
        var projects = solution.Projects;
        var filename = $"{nameof(verify)}.yml".ToFileName();
        File.WriteAllText(filename, verify.content(projects));
        
        filename = $"{nameof(clean_images)}.yml".ToFileName();
        File.WriteAllText(filename, clean_images.content(solution));
        
        filename = $"{nameof(run_sanity_tests)}.yml".ToFileName();
        File.WriteAllText(filename, run_sanity_tests.content(solution));
        
        filename = $"{nameof(release_dev)}.yml".ToFileName();
        File.WriteAllText(filename, release_dev.content(solution));

        filename = $"{nameof(release_preprod)}.yml".ToFileName();
        File.WriteAllText(filename, release_preprod.content(projects));

        filename = $"{nameof(release_prod)}.yml".ToFileName();
        File.WriteAllText(filename, release_prod.content(solution));

        filename = $"{nameof(release_qa)}.yml".ToFileName();
        File.WriteAllText(filename, release_qa.content(solution));

        filename = $"{nameof(release_reuse)}.yml".ToFileName();
        File.WriteAllText(filename, release_reuse.content);

        filename = $"{nameof(transition_jira_issues)}.yml".ToFileName();
        File.WriteAllText(filename, transition_jira_issues.content);
    }
}

public static class Utils
{
    public static string ToFileName(this string name)
    {
        return name.Replace("_", "-");
    }
}