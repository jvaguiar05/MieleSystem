namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// Estatísticas de conexão de um usuário.
/// </summary>
public class ConnectionStatsDto
{
    /// <summary>
    /// Número de tentativas de conexão bem-sucedidas no período.
    /// </summary>
    public int SuccessfulConnections { get; init; }

    /// <summary>
    /// Número de tentativas de conexão falhadas no período.
    /// </summary>
    public int FailedConnections { get; init; }

    /// <summary>
    /// Número de endereços IP únicos utilizados no período.
    /// </summary>
    public int UniqueIpAddresses { get; init; }

    /// <summary>
    /// Data/hora da última conexão bem-sucedida.
    /// </summary>
    public DateTime? LastSuccessfulConnection { get; init; }

    /// <summary>
    /// Data/hora da última tentativa de conexão (independente do resultado).
    /// </summary>
    public DateTime? LastConnectionAttempt { get; init; }

    /// <summary>
    /// Indica se houve atividade suspeita no período analisado.
    /// </summary>
    public bool HasSuspiciousActivity => FailedConnections >= 3 || UniqueIpAddresses >= 5;
}
