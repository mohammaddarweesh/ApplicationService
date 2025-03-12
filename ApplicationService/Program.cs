using AppService.Services.IService;
using AppService.Services.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<IStorageServiceClient, StorageServiceClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// configure the application to run over http
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
