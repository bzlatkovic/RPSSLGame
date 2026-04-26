using System.Text.Json.Serialization;

namespace RPSSLGame.Api.Models.External;

public record RandomNumberResponse(
    [property: JsonPropertyName("random_number")]
    int RandomNumber);