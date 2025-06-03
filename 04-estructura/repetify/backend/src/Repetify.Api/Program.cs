using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using Repetify.Api.Extensions.DependencyInjection;
using Repetify.Api.Handlers;
using Repetify.Api.Middlewares;

namespace Repetify.Api;

internal sealed class Program
{
	static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Configuration.AddUserSecrets<Program>();

		// Register all services
		builder.Services.AddRepetifyDependencies(builder.Configuration);

		// Exception handlers
		builder.Services.AddExceptionHandler<UnexpectedExceptionHandler>();
		builder.Services.AddProblemDetails();

		// HttpClient
		builder.Services.AddHttpClient("RepetifyApi", client =>
		{
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		});
		builder.Services.AddControllers();
		// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
		builder.Services.AddOpenApi();

		var app = builder.Build();

		app.UseExceptionHandler();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
			app.UseSwaggerUi(options =>
			{
				options.DocumentPath = "openapi/v1.json";
			});
		}

		app.UseHttpsRedirection();
		app.UseAuthorization();
		app.UseMiddleware<SlidingExpirationMiddleware>();

		app.MapControllers();

		app.Run();
	}
}
