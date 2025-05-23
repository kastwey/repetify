using Microsoft.EntityFrameworkCore;

namespace Repetify.Infrastructure.Persistence.EfCore.Repositories;

/// <summary>  
/// Base class for repository implementations, providing common functionality for interacting with the database.  
/// </summary>  
public abstract class RepositoryBase(DbContext context)
{
	private const string InMemoryDBProviderName = "Microsoft.EntityFrameworkCore.InMemory";

	/// <summary>  
	/// Determines if the current database provider is an in-memory database.  
	/// </summary>  
	/// <returns>True if the database provider is in-memory; otherwise, false.</returns>  
	protected bool IsInMemoryDb() => context.Database.ProviderName?.Equals(InMemoryDBProviderName, StringComparison.Ordinal) ?? false;
}
