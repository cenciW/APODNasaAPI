using APODNasaAPI.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

//http not https    
builder.WebHost.ConfigureKestrel(serverOpt =>
{
    serverOpt.ListenAnyIP(5000);
    serverOpt.ListenLocalhost(5001);

});

// Add services to the container.

builder.Services.AddSingleton<APODRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

//allow any origin
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .WithHeaders(HeaderNames.AccessControlAllowHeaders, "Content-Type")
                .AllowAnyMethod();

        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
        //app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

//app.UseHttpsRedirection();
//app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthorization();


app.MapControllers();

app.Run();



