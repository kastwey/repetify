using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Repetify.Infrastructure.Persistence.EfCore.Entities;

namespace Repetify.Infrastructure.Persistence.EfCore.EntityConfigurations;

/// <summary>
/// Configures the UserEntity model.
/// </summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
	/// <summary>
	/// Configures the UserEntity model properties and relationships.
	/// </summary>
	/// <param name="builder">The builder to be used to configure the DeckEntity.</param>
	public void Configure(EntityTypeBuilder<UserEntity> builder)
	{
		builder.ToTable("Users");
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Email)
			.IsRequired()
			.HasMaxLength(100);
		builder.Property(u => u.Username)
					.IsRequired()
					.HasMaxLength(100);
	}
}
