using Microsoft.EntityFrameworkCore;

using Repetify.Infrastructure.Persistence.EfCore.Context;

namespace Repetify.Infrastructure.Persistence.EfCore.UnitTests.Helpers;

internal static class TestsHelper
{
	internal static RepetifyDbContext CreateInMemoryDbContext()
	{
		var options = new DbContextOptionsBuilder<RepetifyDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new(options);
	}
}