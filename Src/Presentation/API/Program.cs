using API.Filters;
using API.Service;
using Application;
using Application.Common.Interfaces.Identity;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;

const string appsettingsTestPath = ".//appsettings.test.json"; 
var builder = WebApplication.CreateBuilder(args);
if (!File.Exists(appsettingsTestPath))
{
    builder.Configuration.AddAzureKeyVault(builder.Configuration.GetValue<string>("KeyVault"));
}


// Add services to the container.
builder.Services.AddControllers(opt =>
{
    if (!File.Exists(appsettingsTestPath))
    {
        opt.Filters.Add<ApiExceptionFilterAttribute>();
    }
}).AddFluentValidation(x => x.AutomaticValidationEnabled = false);
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();
//app.AddSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }