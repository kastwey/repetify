using Microsoft.EntityFrameworkCore;

using Repetify.Crosscutting;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Entities;
using Repetify.Infrastructure.Persistence.EfCore.Context;
using Repetify.Infrastructure.Persistence.EfCore.Extensions.Mappers;

using System.Diagnostics.CodeAnalysis;

namespace Repetify.Infrastructure.Persistence.EfCore.Repositories;

public class UserRepository(RepetifyDbContext dbContext) : RepositoryBase(dbContext), IUserRepository
{
	private readonly RepetifyDbContext _context = dbContext;

	/// <inheritdoc />  
	public async Task<Result<User>> GetUserByEmailAsync(string email)
	{
		var userEntity = await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Email == email)
			.ConfigureAwait(false);

		return userEntity is null
			? ResultFactory.NotFound<User>($"User with email {email} not found.")
			: ResultFactory.Success(userEntity.ToDomain());
	}

	///  <inheritdoc/>
	[SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "Not supported in EF Core")]
	public async Task<Result<bool>> EmailAlreadyExistsAsync(Guid userId, string email)
	{
		ArgumentNullException.ThrowIfNull(email);
		return ResultFactory.Success(await _context.Users
			.AnyAsync(u => u.Id != userId && u.Email.Equals(email))
			.ConfigureAwait(false));
	}

	///  <inheritdoc/>
	[SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "Not supported in EF Core")]
	public async Task<Result<bool>> UsernameAlreadyExistsAsync(Guid userId, string username)
	{
		ArgumentNullException.ThrowIfNull(username);
		return ResultFactory.Success(await _context.Users
			.AnyAsync(u => u.Id != userId && u.Username.Equals(username))
			.ConfigureAwait(false));
	}

	/// <inheritdoc />  
	public async Task<Result> AddUserAsync(User user)
	{
		ArgumentNullException.ThrowIfNull(user);
		await _context.Users.AddAsync(user.ToDataEntity()).ConfigureAwait(false);
		return ResultFactory.Success();
	}

	/// <inheritdoc />  
	public async Task<Result> UpdateUserAsync(User user)
	{
		var userEntity = await _context.Users.FindAsync(user.Id).ConfigureAwait(false);
		if (userEntity is null)
		{
			return ResultFactory.NotFound($"User with ID {user.Id} not found.");
		}

		userEntity.UpdateFromDomain(user);
		return ResultFactory.Success();
	}

	/// <inheritdoc />  
	public async Task<Result> DeleteUserAsync(Guid userId)
	{
		if (!await _context.Users.AnyAsync(u => u.Id == userId).ConfigureAwait(false))
		{
			return ResultFactory.NotFound($"User with ID {userId} not found.");
		}

		if (IsInMemoryDb())
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
			_context.Users.Remove(user!);
		}
		else
		{
			await _context.Users.Where(u => u.Id == userId).ExecuteDeleteAsync().ConfigureAwait(false);
		}

		return ResultFactory.Success();
	}

	/// <inheritdoc />  
	public async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync().ConfigureAwait(false);
	}
}
