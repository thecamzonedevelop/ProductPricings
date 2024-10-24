

using MenuLib;
using ProductLib;
using RestClientLib;
using System;
using System.Text.Json;
using ProductLib.Models;

namespace ProductClient;
public static class ProductHelper
{
    public static string BaseUri { get; }
    private static string ProductRoute { get; }
    static ProductHelper()
    {
        var json = File.ReadAllText("appsettings.json");
        var config = JsonSerializer.Deserialize<AppConfig>(json);
        BaseUri = config?.ProductServiceUrl ?? string.Empty;
        ProductRoute = config?.BookRoute ?? string.Empty;
    }
    public static MenuBank MenuBank { get; set; } = new MenuBank()
    {
        Title = "Products",
        Menus = new List<Menu>()
        {
            new Menu(){ Text= "Viewing", Action=ViewingProducts},
            new Menu(){ Text= "Creating", Action=CreatingProducts},
            new Menu(){ Text= "Updating", Action=UpdatingProducts},
            new Menu(){ Text= "Deleting", Action=DeletingProducts},
            new Menu(){ Text= "Exiting", Action = ExitingProgram}
        }
    };


    private static void ViewingProducts()
    {
        Console.WriteLine("\n[Viewing Products]");
        Task.Run(async () =>
        {
            using (var client = new RestClient(BaseUri))
            {
                var endpoint = $"{ProductRoute}";
                try
                {
                    var result = await client.GetAsync<Result<List<ProductResponse>>>(endpoint);
                    var all = result?.Data??[];
                    var count = result?.Data?.Count()??0;
                    Console.WriteLine($"Products: {count}");
                    if (count > 0)
                    {
                        Console.WriteLine($"{"Id",-36} {"Code",-10} {"Name",-30} {"Category",-20}");
                        Console.WriteLine(new string('=', 36 + 1 + 10 + 1 + 30 + 1 + 20));
                        foreach (var prd in all)
                        {
                            Console.WriteLine($"{prd.Id,-36} {prd.Code,-10} {prd.Name,-30} {prd.Category,-20}");
                        }
                    }   
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error>{ex.Message}");
                }
            }
        }).Wait();
    }

    private static void CreatingProducts()
    {
        Console.WriteLine("\n[Creating Products]");
        while (true)
        {
            Task.Run(async () =>
            {
                var req = GetCreateProduct();
                if (req != null)
                {
                    using (var client = new RestClient(BaseUri))
                    {
                        var endpoint = $"{ProductRoute}";
                        try
                        {
                            var result = await client.PostAsync<ProductCreateReq, Result<string?>>(endpoint, req);
                            if (result?.Succeded ?? false)
                            {
                                Console.WriteLine($"Succesfully creata a new product with id, {result.Data}");
                            }
                            else
                            {
                                Console.WriteLine($"{result?.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error>{ex.Message}");
                        }
                    }
                }
            }).Wait();

            Console.WriteLine();
            if (WaitForEscPressed("ESC to stop or any key for more creating...")) break;
        }
    }

    private static void UpdatingProducts()
    {
        Console.WriteLine("\n[Updating Products]");
        while (true)
        {
            Task.Run(async () =>
            {
                Console.Write("Product Id/Code(required): ");
                var key = Console.ReadLine() ?? "";
                using (var client = new RestClient(BaseUri))
                {
                    try
                    {
                        //Getting an existing product of given key
                        string endpoint = $"{ProductRoute}/{key}";
                        var result = await client.GetAsync<Result<ProductResponse?>>(endpoint);
                        

                        if (result?.Succeded ?? false) //found
                        {
                            var prd = result.Data;
                            Console.Write($"Current name:{prd?.Name} > New name (optional)  : ");
                            var name = Console.ReadLine();

                            Console.WriteLine($"Category available: {Enum.GetNames<Category>().Aggregate((a, b) => a + ", " + b)}");
                            Console.Write($"Current category:{prd?.Category} > New category: ");
                            var category = Console.ReadLine();

                            var req = new ProductUpdateReq()
                            {
                                Key = prd?.Id ?? "",
                                Name = name,
                                Category = category
                            };

                            //Updating an exiting product
                            endpoint = $"{ProductRoute}";
                            var result2 = await client.PutAsync<ProductUpdateReq, Result<bool>>(endpoint, req);
                            Console.WriteLine($"{result2?.Message}");
                        }
                        else //not found
                        {
                            Console.WriteLine($"{result?.Message}");
                            //
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error > {ex.Message}");
                    }
                }
            }).Wait();
           
            Console.WriteLine();
            if (WaitForEscPressed("ESC to stop or any key for more updating...")) break;
        }
    }

    private static void DeletingProducts()
    {
        Console.WriteLine("\n[Deleting Product]");
        while (true)
        {
            Task.Run(async () =>
            {
                Console.Write("Product Id/Code: ");
                var key = Console.ReadLine() ?? "";
                using (var client = new RestClient(BaseUri))
                {
                    try
                    {
                        string endpoint = $"{ProductRoute}/{key}";
                        var result = await client.DeleteAsync<Result<bool>>(endpoint);
                        if (result?.Succeded ?? false)
                        {
                            Console.WriteLine($"Successfully delete the product with id/code, {key}");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to delete a book with id/code, {key}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error> {ex.Message}");
                    }

                }
            }).Wait();
            Console.WriteLine();
            if (WaitForEscPressed("ESC to stop or any key for more deleting ...")) break;
        }
    }

    public static void ExitingProgram()
    {
        Console.WriteLine("\n[Exiting Program]");
        Environment.Exit(0);
    }

    private static bool WaitForEscPressed(string text)
    {
        Console.Write(text); ;
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        Console.WriteLine();
        return keyInfo.Key == ConsoleKey.Escape;
    }
   
    private static ProductCreateReq? GetCreateProduct()
    {
        Console.WriteLine($"Category available: {Enum.GetNames<Category>().Aggregate((a, b) => a + ", " + b)}");
        Console.Write("Product(code/name/category): ");
        string data = Console.ReadLine() ?? "";
        var dataParts = data.Split("/");
        if (dataParts.Length < 3)
        {
            Console.WriteLine("Invalid create product's data");
            return null;
        }
        var code = dataParts[0].Trim();
        var name = dataParts[1].Trim();
        var category = dataParts[2].Trim();

        return new ProductCreateReq() { Code = code, Name = name, Category = category };

    }
}

