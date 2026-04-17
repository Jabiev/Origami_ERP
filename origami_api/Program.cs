using Origami.Api.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence;
using Origami.Api.Infrastructure.Persistence.Scaffolded;
using Origami.Api.Infrastructure.Recognition;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.Configure<RecognitionOptions>(builder.Configuration.GetSection(RecognitionOptions.SectionName));

builder.Services.AddDbContext<OrigamiScaffoldDbContext>((serviceProvider, options) =>
{
    var databaseOptions = serviceProvider
        .GetRequiredService<IConfiguration>()
        .GetSection(DatabaseOptions.SectionName)
        .Get<DatabaseOptions>() ?? new DatabaseOptions();

    options.UseNpgsql(DatabaseConnectionStringFactory.Build(databaseOptions));
});

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddScoped<IPythonRecognitionBridge, PythonRecognitionBridge>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("DefaultCors");
app.UseAuthorization();
app.MapControllers();

app.Run();
