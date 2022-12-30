using System.Text.Json.Serialization;

namespace firestarter;

public class SolutionDescription
{
    public List<Project> Projects { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DescriptionVersion Version { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SupportedLegacySystems? LegacySystem { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GitWorkflow GitWorkflow { get; set; }
}

public enum GitWorkflow
{
    TrunkBased,
    Gitflow
}
public enum DescriptionVersion
{
    v1=1
}

public class Project
{
    public string Name { get; set; }
    public string DockerFile => LegacyProperties?.DockerFile ?? $"{Name}/Dockerfile";
    public string ServiceName => LegacyProperties?.ServiceName ?? $"{Name}-service";
    public string TestFilter => LegacyProperties?.TestFilter ?? $"";
    public DeprecatedProperties LegacyProperties { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TechStack Tech { get; set; }
}

public class DeprecatedProperties
{
    public string ContainerName { get; set; }
    public string DockerFile { get; set; } = "Dockerfile";
    public string ServiceName { get; set; }
    public string TestFilter { get; set; }
}

public enum TechStack
{
    invalid,
    dotnet,
    php,
    js,
    legacy_APFE,
    legacy_dotnet
}

public enum SupportedLegacySystems
{
    apfe,
    auction_service
}