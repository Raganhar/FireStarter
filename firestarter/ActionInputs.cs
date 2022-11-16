namespace firestarter;

public class ActionInputs
{
    public string Directory { get; set; } = "";
}

public class SolutionDescription
{
    public TechStack Tech { get; set; }
    public List<Project> Projects { get; set; }
    public DescriptionVersion Version { get; set; }
}

public enum DescriptionVersion
{
    v1
}

public class Project
{
    public string Name { get; set; }
}

public enum TechStack
{
    dotnet,
    php,
    js,
    legacy_APFE
}