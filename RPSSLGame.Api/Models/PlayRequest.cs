using System.ComponentModel;

namespace RPSSLGame.Api.Models;

public record PlayRequest(
    [property: DefaultValue(1)] int? Player);