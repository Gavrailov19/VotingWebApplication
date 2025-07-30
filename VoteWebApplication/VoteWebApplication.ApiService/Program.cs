using SignalR.Orleans;
using VoteWebApplication.ApiService.Voting;

var builder = WebApplication.CreateBuilder(args);

// Service defaults (logging, configuration, etc.)
builder.AddServiceDefaults();

// API error details
builder.Services.AddProblemDetails();

// Swagger/OpenAPI for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MVC controllers
builder.Services.AddControllers();

// SignalR with Orleans backplane
builder.Services.AddSignalR().AddOrleans();

// Orleans silo configuration
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage(SignalROrleansConstants.SIGNALR_ORLEANS_STORAGE_PROVIDER);
    siloBuilder.UseSignalR();
    siloBuilder.RegisterHub<VotingHub>();
});

// CORS policy for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
        policy.WithOrigins("https://localhost:7141")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// Global exception handler
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Voting API V1");
    });
}

app.UseCors("AllowBlazorClient");
app.UseRouting();

app.MapControllers();
app.MapHub<VotingHub>("/votingHub");
app.MapDefaultEndpoints();

app.Run();