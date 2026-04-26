using RPSSLGame.Api.Models.External;

namespace RPSSLGame.Api.Services;

public class RandomNumberService(
    HttpClient httpClient,
    ILogger<RandomNumberService> logger) : IRandomNumberService
{
    public async Task<int> GetRandomNumberAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving random number...");
        var response = await httpClient.GetFromJsonAsync<RandomNumberResponse>(string.Empty, cancellationToken);

        if (response is null or { RandomNumber: <= 0 })
        {
            logger.LogWarning("Random number service returned invalid response");
            throw new InvalidOperationException("Random number service returned invalid response");
        }

        logger.LogDebug("Retrieved random number: {RandomNumber}", response.RandomNumber);
        return response.RandomNumber;
    }
}