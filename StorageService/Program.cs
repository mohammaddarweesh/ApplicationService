using StorageService.Services.IServices;
using StorageService.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IStorageService, StorageService.Services.Services.StorageService>();
builder.Services.AddSingleton<ISignatureService, SignatureService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
// configure the application to run over http
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
