using Microsoft.EntityFrameworkCore;

using Repetify.Infrastructure.Persistence.EfCore.Context;

namespace Repetify.Infrastructure.Persistence.EfCore.Tests.Helpers;

internal static class TestHelpers
{
	internal static RepetifyDbContext CreateInMemoryDbContext()
	{
		var options = new DbContextOptionsBuilder<RepetifyDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new(options);
	}
}