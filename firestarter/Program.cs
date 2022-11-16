using firestarter;
using firestarter.GithubActionModels.Pullrequest;
using Newtonsoft.Json;

using IHost host = Host.CreateDefaultBuilder(args)
    // .ConfigureServices((_, services) => services.AddGitHubActionServices())
    .Build();

static TService Get<TService>(IHost host)
    where TService : notnull =>
    host.Services.GetRequiredService<TService>();

var logger = Get<ILoggerFactory>(host)
    .CreateLogger("DotNet.GitHubAction.Program");

var parser = Default.ParseArguments<ActionInputs>(() => new(), args);
parser.WithNotParsed(
    errors =>
    {
        logger
            .LogError(
                string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        
        Environment.Exit(2);
    });
var done = false;
await parser.WithParsedAsync(async options =>
{
    logger.LogInformation($"Options: {JsonConvert.SerializeObject(options,Formatting.Indented)}");
    await new Logic(logger, options).DoDaThing();
    done = true;
});
logger.LogInformation($"Done: {done}");
Environment.Exit(0);
