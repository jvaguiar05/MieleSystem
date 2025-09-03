namespace MieleSystem.Infrastructure.Identity.Options;

public sealed class BCryptOptions
{
    public int WorkFactor { get; init; } = default!;
}
