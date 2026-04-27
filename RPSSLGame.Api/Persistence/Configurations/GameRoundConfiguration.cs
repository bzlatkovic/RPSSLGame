using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RPSSLGame.Api.Persistence.Entities;

namespace RPSSLGame.Api.Persistence.Configurations;

public class GameRoundConfiguration : IEntityTypeConfiguration<GameRound>
{
    public void Configure(EntityTypeBuilder<GameRound> builder)
    {
        builder.ToTable("game_rounds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.PlayerChoice)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("player_choice");

        builder.Property(x => x.ComputerChoice)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("computer_choice");

        builder.Property(x => x.Result)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("result");

        builder.Property(x => x.PlayedAt)
            .IsRequired()
            .HasColumnName("played_at")
            .HasColumnType("timestamp with time zone");
    }
}