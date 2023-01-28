namespace firestarter;

public enum DeploymentEnvironments
{
    dev,
    stage,
    prod
}

public static class EnumExtension
{
    public static string ToBranch(this DeploymentEnvironments env)
    {
        switch (env)
        {
            case DeploymentEnvironments.dev:
                return "dev";
                break;
            case DeploymentEnvironments.stage:
                return "release";
                break;
            case DeploymentEnvironments.prod:
                return "main";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(env), env, null);
        }
    }
}