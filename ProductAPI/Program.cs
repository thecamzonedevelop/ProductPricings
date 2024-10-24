using ProductAPI;
using ProductLib;
using ProductLib.Context;
using ProductLib.repository;

internal class Program
{
    private static void Main(string[] args)
    {
        const string CORS_POLICY = "CORS_Policy";
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddCors(option =>
        {
            option.AddPolicy(CORS_POLICY, builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        builder.Services.AddScoped<FileContext>(provider =>
        {
            var fileName = builder.Configuration.GetValue<string>("FileSettings:FileName") ?? "products.txt";
            return new FileContext(fileName);
        });
        builder.Services.AddScoped<ProductRepository>();
        builder.Services.AddScoped<ProductService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors(CORS_POLICY);
        app.MapProductEndpoints("Products", CORS_POLICY);

        app.UseHttpsRedirection();

        //Initializing products
        using (var scope = app.Services.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<ProductService>();
            service.Initialize();
        }
        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
