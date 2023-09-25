using MessageBus.Engine.IoC;
using MessagePublisher.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMessageBus(builder.Configuration);
builder.Services.AddErrorFilter();
builder.Services.AddNewtonsoftJson();
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();