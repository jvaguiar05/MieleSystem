namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class BCryptOptions
{
    public int WorkFactor { get; init; } =
        Environment.GetEnvironmentVariable("BCRYPT_WORK_FACTOR") is string wf
        && int.TryParse(wf, out var value)
            ? value
            : 10; // padrão: 10
}
