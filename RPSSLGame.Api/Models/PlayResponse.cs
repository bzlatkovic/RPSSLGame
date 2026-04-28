using System.ComponentModel;

namespace RPSSLGame.Api.Models;

public record PlayResponse(string Results, [property: DefaultValue(1)] int Player, [property: DefaultValue(1)] int Computer);