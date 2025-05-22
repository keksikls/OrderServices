using Microsoft.AspNetCore.HttpLogging;
using OrderService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpLogging(opt =>
{
    opt.LoggingFields = HttpLoggingFields.RequestBody | HttpLoggingFields.RequestHeaders | HttpLoggingFields.Duration |
                        HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseBody | HttpLoggingFields.ResponseHeaders;
});

builder.AddBearerAuthentication();
builder.AddOptions();
builder.AddBackgroundService();
builder.AddElastic();
builder.AddSwagger();
builder.AddData();
builder.AddApplicationServices();

var app = builder.Build();

app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();