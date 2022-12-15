namespace firestarter;

public class ActionInputs
{
    [Option('d',"directory", HelpText = "root directory of solution folder")]
    public string Directory { get; set; } = "";
}
