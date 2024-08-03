using Microsoft.AspNetCore.HttpLogging;
using TestAPITask.Helpers;
using TestAPITask.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddHttpClient("Products");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(logging => {
    logging.LoggingFields = HttpLoggingFields.Duration 
        | HttpLoggingFields.RequestQuery
        | HttpLoggingFields.RequestPath
        | HttpLoggingFields.RequestMethod
        | HttpLoggingFields.ResponsePropertiesAndHeaders
        | HttpLoggingFields.ResponseBody;
});

var app = builder.Build();

//app.UseHttpLogging();
app.MapWhen(
    context => context.Request.Path.StartsWithSegments("/api/"),
    builder => builder.UseHttpLogging()
);

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
