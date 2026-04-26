namespace RPSSLGame.Api.Services;

public interface IRandomNumberService
{
    Task<int> GetRandomNumberAsync(CancellationToken cancellationToken);
}