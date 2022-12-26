using firestarter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace tests;

[Ignore("integration tests")]
public class FireStarterTest
{
    private SolutionDescription _solutionDescription = new SolutionDescription
    {
        Version = DescriptionVersion.v1,
        Projects = new List<Project>
        {
            new Project
            {
                Name = "SomeAPi",Tech = TechStack.dotnet,
            },
            new Project
            {
                Name = "SomeBackgroundWorker",Tech = TechStack.dotnet,
            }
        }
    };

    [Test]
    public async Task GenerateEverything()
    {
        var conf = new ActionInputs
        {
            Directory = "C:\\Code\\auto-generate-pipeline-files",
        };
        // Directory.Delete(conf.Directory, true);
        Directory.CreateDirectory(conf.Directory);
        
        Directory.CreateDirectory(Path.Combine(conf.Directory, ".ap"));
        File.WriteAllText(Path.Combine(conf.Directory,Logic._solutionDescriptionPath), JsonConvert.SerializeObject(_solutionDescription, Formatting.Indented));
        
        
        await new Logic(NSubstitute.Substitute.For<ILogger>(), conf).DoDaThing();
    }
}