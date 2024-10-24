namespace ProductClient;
using ProductLib;
using System;

internal class Program
{
    static void Main(string[] args)
    {
    Console.WriteLine("Product Management");
    ProductHelper.MenuBank.MenuSimulate(() => { Console.WriteLine(); });
    }
}
