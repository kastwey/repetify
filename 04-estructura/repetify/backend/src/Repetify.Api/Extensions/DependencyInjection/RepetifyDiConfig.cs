using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Repetify.Api.Config;
using Repetify.Application.Abstractions.Services;
using Repetify.Application.Services;
using Repetify.AuthPlatform;
using Repetify.AuthPlatform.Abstractions;
using Repetify.AuthPlatform.Abstractions.IdentityProviders;
using Repetify.AuthPlatform.Config;
using Repetify.AuthPlatform.Config.Google;
using Repetify.AuthPlatform.Config.Microsoft;
using Repetify.AuthPlatform.IdentityProviders;
using Repetify.Domain.Abstractions;
using Repetify.Domain.Abstractions.Repositories;
using Repetify.Domain.Abstractions.Services;
using Repetify.Domain.Services;
using Repetify.Infrastructure.Persistence.EfCore.Context;
using Repetify.Infrastructure.Persistence.EfCore.Repositories;
using Repetify.Infrastructure.Time;

using System.Text;

namespace Repetify.Api.Extensions.DependencyInjection;

internal static class RepetifyDiConfig
{

	public static IServiceCollection AddRepetifyDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDomainDependencies()
			.AddInfrastructureDependencies()
			.AddPersistenceDependencies(configuration)
			.AddApplicationDependencies()
			.AddApplicationConfig(configuration)
			.AddJwtAuthentication(configuration);

		return services;
	}

	private static IServiceCollection AddDomainDependencies(this IServiceCollection services)
	{
		services.AddScoped<IDeckValidator, DeckValidator>();
		services.AddScoped<IUserValidator, UserValidator>();
		services.AddSingleton<ICardReviewService, CardReviewService>();
		return services;
	}

	private static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
	{
		services.AddSingleton<IClock, SystemClock>();
		services.AddSingleton<IJwtService, JwtService>();
		services.AddScoped<IGoogleOauthService, GoogleOauthService>();
		services.AddScoped<IMicrosoftOauthService, MicrosoftOauthService>();

		return services;
	}

	private static IServiceCollection AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<RepetifyDbContext>(options =>
			options.UseSqlServer(connectionString)
		);

		services.AddScoped<IDeckRepository, DeckRepository>();
		services.AddScoped<IUserRepository, UserRepository>();

		return services;
	}

	private static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
	{
		services.AddScoped<IDeckAppService, DeckAppService>();
		services.AddScoped<IUserAppService, UserAppService>();

		return services;
	}

	private static IServiceCollection AddApplicationConfig(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.ConfigSection));
		services.Configure<GoogleOauthConfig>(configuration.GetSection(GoogleOauthConfig.ConfigSection));
		services.Configure<MicrosoftOauthConfig>(configuration.GetSection(MicrosoftOauthConfig.ConfigSection));
		services.Configure<FrontendConfig>(configuration.GetSection(FrontendConfig.ConfigSection));

		return services;
	}

	private static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var jwtConfig = configuration.GetSection(JwtConfig.ConfigSection).Get<JwtConfig>()!;
		serviceCollection.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = (context) =>
				{
					var cookie = context.Request.Cookies["AuthToken"];
					if (cookie != null)
					{
						context.Token = cookie;
					}

					return Task.CompletedTask;
				}
			};
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtConfig.Issuer,
				ValidAudience = jwtConfig.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SigningKey))
			};
		});
		return serviceCollection;
	}
}