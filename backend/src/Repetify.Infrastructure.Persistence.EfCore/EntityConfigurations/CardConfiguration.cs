using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Repetify.Infrastructure.Persistence.EfCore.Entities;

namespace Repetify.Infrastructure.Persistence.EfCore.EntityConfigurations;

/// <summary>
/// Configures the CardEntity model.
/// </summary>
internal sealed class CardConfiguration : IEntityTypeConfiguration<CardEntity>
{
	/// <summary>
	/// Configures the CardEntity model properties and relationships.
	/// </summary>
	/// <param name="builder">The builder to be used to configure the DeckEntity.</param>
	public void Configure(EntityTypeBuilder<CardEntity> builder)
	{
		builder.ToTable("Cards");
		builder.HasKey(c => c.Id);
		builder.Property(c => c.OriginalWord)
			.IsRequired()
			.HasMaxLength(500);
		builder
			.Property(c => c.TranslatedWord)
			.IsRequired()
			.HasMaxLength(500);
	}
}