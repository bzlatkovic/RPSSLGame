using RPSSLGame.Api.Domain;

namespace RPSSLGame.Api.Models;

public class ChoiceDto
{
    public ChoiceDto(Choice choice)
    {
        Id = (int)choice;
        Name = choice.ToString().ToLower();
    }

    public int Id { get; }
    public string Name { get; }
}