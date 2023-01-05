using firestarter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace tests;

// [Ignore("integration test")]
public class FireStarterTest_legacyFlows
{
    [Test]
    public async Task GenerateEverything()
    {
        var conf = new ActionInputs
        {
            // Directory = "C:\\Code\\NotificationService",
            // Directory = "C:\\Code\\ManagedService",
            // Directory = "C:\\Code\\AuctionService",
            Directory = "C:\\Code\\ProductService",
            // Directory = "C:\\Code\\ap-frontend",
            // Directory = "C:\\Code\\ghcr-ecr-deploy-push",
            // Directory = "C:\\Code\\tbd-integration-api",
            //.2
        };
     
        await new Logic(NSubstitute.Substitute.For<ILogger>(), conf).DoDaThing();
    }
}