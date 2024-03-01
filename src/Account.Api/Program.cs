using Account.Api.Data;
using Account.Api.Services;

using Microsoft.EntityFrameworkCore;

using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AccountApiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountApiContext")
        ?? throw new InvalidOperationException("Connection string 'AccountApiContext' not found.")));

builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<ITransacaoValidacaoService, TransacaoValidacaoService>();

var configurationOptions = ConfigurationOptions.Parse(builder.Configuration["REDIS_HOST"]!);

var multiplexer = new List<RedLockMultiplexer> {
    ConnectionMultiplexer.Connect(configurationOptions)
};

var redlockFactory = RedLockFactory.Create(multiplexer);

builder.Services.AddSingleton<IDistributedLockFactory>(redlockFactory);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program
{ }