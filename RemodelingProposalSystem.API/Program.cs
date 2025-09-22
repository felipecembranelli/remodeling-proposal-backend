using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Core.Services;
using RemodelingProposalSystem.Infrastructure.Data;
using RemodelingProposalSystem.Infrastructure.Repositories;
using RemodelingProposalSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;
using RemodelingProposalSystem.Core.LLM;
using RemodelingProposalSystem.Infrastructure.LLM;
using RemodelingProposalSystem.Core.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register the DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register Repositories
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IProposalRepository, ProposalRepository>();
//builder.Services.AddScoped<IProposalRepository, ProposalRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IPricingRepository, PricingRepository>();

// Register services
builder.Services.AddOpenAIService(settings => { settings.ApiKey = Environment.GetEnvironmentVariable("MY_OPEN_AI_API_KEY"); });

// Inject the desired LLModel
//builder.Services.AddScoped<ILLMService, RemodelingProposalSystem.Infrastructure.LLM.GemmaService>();
builder.Services.AddScoped<ILLMService, RemodelingProposalSystem.Infrastructure.LLM.MockLLMService>();


builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IProposalService, ProposalService>();

// Add services to the container.
builder.Services.AddScoped<RemodelingProposalSystem.Infrastructure.LLM.OpenAIService>();
builder.Services.AddScoped<GemmaService>();
builder.Services.AddScoped<LlamaService>();
builder.Services.AddHttpClient();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3001")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();


// Need for integration tests with WebApplicationFactory 
// https://stackoverflow.com/questions/69991983/deps-file-missing-for-dotnet-6-integration-tests
public partial class Program { }