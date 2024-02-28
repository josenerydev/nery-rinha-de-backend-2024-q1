using Account.Api.Data;
using Account.Api.Models;
using Account.Api.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountApiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountApiContext") ?? throw new InvalidOperationException("Connection string 'AccountApiContext' not found.")));

// Add services to the container.
builder.Services.AddScoped<TransacaoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Aplica migrações e inicializa o banco de dados com clientes iniciais
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AccountApiContext>();
    InitializeDatabase(dbContext);
}

app.Run();

void InitializeDatabase(AccountApiContext context)
{
    // Aplica migrações pendentes
    context.Database.Migrate();

    // Verifica se já existem clientes
    if (!context.Clientes.Any())
    {
        var clientes = new List<Cliente>
        {
            new Cliente { Id = 1, Limite = 100000, Saldo = 0 },
            new Cliente { Id = 2, Limite = 80000, Saldo = 0 },
            new Cliente { Id = 3, Limite = 1000000, Saldo = 0 },
            new Cliente { Id = 4, Limite = 10000000, Saldo = 0 },
            new Cliente { Id = 5, Limite = 500000, Saldo = 0 }
        };

        context.Clientes.AddRange(clientes);
        context.SaveChanges();
    }
}