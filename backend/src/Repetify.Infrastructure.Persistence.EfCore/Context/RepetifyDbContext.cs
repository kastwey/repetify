using Microsoft.EntityFrameworkCore;

using Repetify.Infrastructure.Persistence.EfCore.Entities;

using System.Diagnostics.CodeAnalysis;

namespace Repetify.Infrastructure.Persistence.EfCore.Context;

/// <summary>
/// Represents the database context for the Repetify application.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "The call to OnModelCreating is made by EF Core, which checks nulls internally.")]
public class RepetifyDbContext : DbContext
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RepetifyDbContext"/> class.
	/// </summary>
	/// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
	public RepetifyDbContext(DbContextOptions options) : base(options)
	{
	}

	/// <summary>
	/// Gets or sets the DbSet for Deck entities.
	/// </summary>
	public DbSet<DeckEntity> Decks { get; set; }

	/// <summary>
	/// Gets or sets the DbSet for Card entities.
	/// </summary>
	public DbSet<CardEntity> Cards { get; set; }

	/// <summary>
	/// Gets or sets the DbSet for User entities.
	/// </summary>
	public DbSet<UserEntity> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepetifyDbContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}
}
