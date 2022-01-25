using Authentication.Infrastructure;
using Authentication.Interfaces;
using Authentication.Providers;
using Authentication.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CiunexDbContext>(options =>
    options.UseNpgsql(builder.Configuration["CiunexDbConnectionString"]));

builder.Services.AddScoped<IUsersProvider, UsersProvider>();
builder.Services.AddScoped<IUsersService, UsersService>();

#endregion

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();