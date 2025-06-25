

using Exatek.Application.Interfaces;
using Exatek.Domain.Interfaces;
using Exatek.Infrastructure.DataAccess;
using Exatek.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Dynamic Repository Registration
var repositoryTypes = typeof(GenericRepository<>).Assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
    .ToList();

foreach (var repositoryType in repositoryTypes)
{
    var interfaceType = repositoryType.GetInterfaces()
        .FirstOrDefault(i => i.Name == $"I{repositoryType.Name}");

    if (interfaceType != null)
    {
        builder.Services.AddScoped(interfaceType, repositoryType);
    }
}

// Alternative dynamic approach using reflection
// This scans for all repository interfaces and implementations
var repositoryAssembly = typeof(IGenericRepository<>).Assembly;
var repositoryInterfaces = repositoryAssembly.GetTypes()
    .Where(t => t.IsInterface && t.Name.EndsWith("Repository") && t.Name.StartsWith("I"))
    .ToList();

foreach (var interfaceType in repositoryInterfaces)
{
    var implementationType = repositoryAssembly.GetTypes()
        .FirstOrDefault(t => t.IsClass && !t.IsAbstract &&
                           interfaceType.IsAssignableFrom(t) &&
                           t.Name == interfaceType.Name.Substring(1)); // Remove 'I' prefix

    if (implementationType != null)
    {
        builder.Services.AddScoped(interfaceType, implementationType);
    }
}

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Dynamic Service Registration
var serviceTypes = typeof(IService).Assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
    .ToList();

foreach (var serviceType in serviceTypes)
{
    var interfaceType = serviceType.GetInterfaces()
        .FirstOrDefault(i => i.Name == $"I{serviceType.Name}");

    if (interfaceType != null)
    {
        builder.Services.AddScoped(interfaceType, serviceType);
    }
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
