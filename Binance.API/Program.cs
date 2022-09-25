using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Binance.API.Extensions.Configuration;
using Binance.Infrastructure;
using MediatR;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

const string CorsPolicyName = "pub";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddXmlDataContractSerializerFormatters()
    .AddFluentValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddContext(builder.Configuration);

builder.Services.AddSettings(builder.Configuration);

builder.Services.RegisterServices();
builder.Services.RegisterMediatR();

builder.Services.AddMemoryCache(); 
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddBackgroundServices();

builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddCors(p => p.AddPolicy(CorsPolicyName, builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddWatchDog();
builder.AddSerilog();


var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseCors(CorsPolicyName);

app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseWatchdog(builder.Configuration);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UpdateDatabase<BinanceContext>();

app.Run();