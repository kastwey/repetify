using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Repetify.Infrastructure.Persistence.EfCore.Entities;

namespace Repetify.Infrastructure.Persistence.EfCore.EntityConfigurations;

/// <summary>
/// Configures the DeckEntity model.
/// </summary>
internal sealed class DeckConfiguration : IEntityTypeConfiguration<DeckEntity>
{
	/// <summary>
	/// Configures the DeckEntity model properties and relationships.
	/// </summary>
	/// <param name="builder">The builder to be used to configure the DeckEntity.</param>
	public void Configure(EntityTypeBuilder<DeckEntity> builder)
	{
		builder.ToTable("Decks");
		builder.HasKey(d => d.Id);
		builder.Property(d => d.Name)
			.IsRequired()
			.HasMaxLength(100);
		builder.Property(d => d.Description)
			.HasMaxLength(500);
		builder.Property(d => d.OriginalLanguage)
			.IsRequired()
			.HasMaxLength(50);
		builder.Property(d => d.TranslatedLanguage)
			.IsRequired()
			.HasMaxLength(50);
		builder.HasOne(d => d.User)
			.WithMany(u => u.Decks)
			.HasForeignKey(d => d.UserId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasMany(d => d.Cards)
			.WithOne(c => c.Deck)
			.HasForeignKey(c => c.DeckId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
