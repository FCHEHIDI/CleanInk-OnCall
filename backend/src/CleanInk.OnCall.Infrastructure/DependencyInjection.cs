using CleanInk.OnCall.Application.Common.Interfaces;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.AI;
using CleanInk.OnCall.Infrastructure.AI.Agents;
using CleanInk.OnCall.Infrastructure.AI.AzureOpenAI;
using CleanInk.OnCall.Infrastructure.AI.Claude;
using CleanInk.OnCall.Infrastructure.Auth;
using CleanInk.OnCall.Infrastructure.Persistence;
using CleanInk.OnCall.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanInk.OnCall.Infrastructure;

/// <summary>
/// Extension methods to register the Infrastructure layer services into the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core, repositories, AI clients, agents, auth services, and the orchestrator.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration (appsettings.json, env vars).</param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── EF Core + PostgreSQL ─────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // ── Repositories ─────────────────────────────────────────────────────────
        services.AddScoped<ICallRepository, CallRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();

        // ── Auth services ─────────────────────────────────────────────────────────
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        // ── Claude AI ────────────────────────────────────────────────────────────
        services.Configure<ClaudeSettings>(configuration.GetSection(ClaudeSettings.SectionName));

        services.AddHttpClient<ClaudeClient>((sp, client) =>
        {
            var settings = configuration.GetSection(ClaudeSettings.SectionName).Get<ClaudeSettings>()
                ?? new ClaudeSettings();
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", settings.ApiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        // ── Azure OpenAI ─────────────────────────────────────────────────────────
        services.Configure<AzureOpenAiSettings>(
            configuration.GetSection(AzureOpenAiSettings.SectionName));

        services.AddHttpClient<AzureOpenAiClient>((sp, client) =>
        {
            var settings = configuration.GetSection(AzureOpenAiSettings.SectionName)
                .Get<AzureOpenAiSettings>() ?? new AzureOpenAiSettings();
            client.BaseAddress = new Uri(settings.Endpoint);
            client.DefaultRequestHeaders.Add("api-key", settings.ApiKey);
        });

        // ── AI Service (Claude as primary) ────────────────────────────────────────
        services.AddScoped<IAiService, ClaudeAiService>();

        // ── Agents ───────────────────────────────────────────────────────────────
        services.AddScoped<IAgent, TriageAgent>();
        services.AddScoped<IAgent, SummaryAgent>();
        services.AddScoped<IAgent, ComplianceAgent>();
        services.AddScoped<AgentOrchestrator>();

        return services;
    }
}
