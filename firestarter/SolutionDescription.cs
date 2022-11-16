using System.Text.Json.Serialization;

namespace firestarter;

public class SolutionDescription
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TechStack Tech { get; set; }
    public List<Project> Projects { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DescriptionVersion Version { get; set; }
}

public enum DescriptionVersion
{
    v1
}

public class Project
{
    public string Name { get; set; }
    public string ContainerName { get; set; }
    public string DockerFile { get; set; } = "DockerFile";
}

public enum TechStack
{
    dotnet,
    php,
    js,
    legacy_APFE
}