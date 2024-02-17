using Azure.Identity;
using Microsoft.Extensions.Azure;
using UploadingDocxAPI.Services;

namespace UploadingDocxAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<AzureBlobService>();
        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(config.GetSection("AzureWebJobsStorage"));
            clientBuilder.UseCredential(new DefaultAzureCredential());

        });
        var app = builder.Build();
        
        app.UseCors(builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
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
    }
}