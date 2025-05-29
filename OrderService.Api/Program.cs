using Microsoft.AspNetCore.HttpLogging;
using OrderService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders Api v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();