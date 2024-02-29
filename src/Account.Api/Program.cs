using Account.Api.Data;
using Account.Api.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountApiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountApiContext") ?? throw new InvalidOperationException("Connection string 'AccountApiContext' not found.")));

builder.Services.AddScoped<ITransacaoService, TransacaoService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();