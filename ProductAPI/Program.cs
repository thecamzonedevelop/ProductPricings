using Microsoft.EntityFrameworkCore;
using ProductAPI;
using ProductLib;
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

        builder.Services.AddScoped<string>(provider =>
        {
            return  builder.Configuration.GetValue<string>("FileSettings:FileName") ?? "products.txt";
        });

        builder.Services.AddDbContext<DbContext, FileDbContext>(options =>
        {
            options.UseInMemoryDatabase("ProductDb");
        });


        builder.Services.AddScoped<DbContext, FileDbContext>(provider =>
        {
            var context = new FileDbContext(
                                provider.GetRequiredService<DbContextOptions<FileDbContext>>(),
                                provider.GetRequiredService<string>()
                            );
            return context;
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
            var contextService = scope.ServiceProvider.GetRequiredService<FileDbContext>();
            contextService.LoadDataAsync().Wait();
            var prdService = scope.ServiceProvider.GetRequiredService<ProductService>();
            prdService.Initialize();
        }
        app.Run();
    }
}

