using System.ComponentModel;
using System.Text.Json.Serialization;
using RPSSLGame.Api.Domain;

namespace RPSSLGame.Api.Models;

public class ChoiceDto
{
    public ChoiceDto(Choice choice)
    {
        Id = (int)choice;
        Name = choice.ToString().ToLower();
    }

    [JsonConstructor]
    public ChoiceDto(int id, string name)
    {
        Id = id;
        Name = name;
    }

    [DefaultValue(1)] public int Id { get; }

    public string Name { get; }
}