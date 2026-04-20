using practice.API.ExceptionHandlers;
using practice.API.Middleware;
using practice.API.Services;
using practice.Application.Features.Transactions.Commands.ProcessTransfer;
using practice.Application.Interfaces;
using practice.Infrastructure.Data;
using practice.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register .NET 8 specific IExceptionHandler paradigm explicitly
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Natively maps standard problem responses across built-in routines

// Register Application CQRS 
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessTransferCommand).Assembly));

// Register Database & Dapper Implementations directly matching schema
builder.Services.AddScoped<System.Data.IDbConnection>(sp =>
    new Microsoft.Data.SqlClient.SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, DapperUnitOfWork>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
builder.Services.AddScoped<IAuditService, ConsoleAuditService>(); // Tying loop for missing audit mapping

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enforce standard exception handling mapping 
app.UseExceptionHandler();

// Apply idempotent checks strictly on transactional boundaries before controllers 
app.UseMiddleware<IdempotencyMiddleware>();

app.MapControllers();

app.Run();