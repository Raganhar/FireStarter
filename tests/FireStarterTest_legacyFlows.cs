using firestarter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace tests;

public class FireStarterTest_legacyFlows
{
    [Test]
    public async Task GenerateEverything()
    {
        var conf = new ActionInputs
        {
            Directory = "C:\\Code\\ap-frontend",
        };
     
        await new Logic(NSubstitute.Substitute.For<ILogger>(), conf).DoDaThing();
    }
}