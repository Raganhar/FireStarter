using firestarter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace tests;

public class FireStarterTest
{
    private SolutionDescription _solutionDescription = new SolutionDescription
    {
        Tech = TechStack.dotnet, Version = DescriptionVersion.v1,
        Projects = new List<Project>
        {
            new Project
            {
                Name = "SomeAPi"
            },
            new Project
            {
                Name = "SomeBackgroundWorker"
            }
        }
    };

    [Test]
    public async Task GenerateEverything()
    {
        var conf = new ActionInputs
        {
            Directory = "C:\\Code\\Random",
        };
        // Directory.Delete(conf.Directory, true);
        Directory.CreateDirectory(conf.Directory);
        
        Directory.CreateDirectory(Path.Combine(conf.Directory, ".ap"));
        File.WriteAllText(Path.Combine(conf.Directory,Logic._solutionDescriptionPath), JsonConvert.SerializeObject(_solutionDescription, Formatting.Indented));
        
        
        await new Logic(NSubstitute.Substitute.For<ILogger>(), conf).DoDaThing();
    }
}