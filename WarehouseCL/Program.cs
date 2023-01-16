// See https://aka.ms/new-console-template for more information
using Cassandra;
using CassandraWarehouse;
using WarehouseCL;

Menu menu = new Menu(new BackendSession("localhost", "warehouse"));

menu.Run();
//Console.WriteLine("Hello, World!");
//BackendSession session = new BackendSession("localhost", "warehouse");
//var wares = session.GetWares();
//Console.WriteLine("Wares:\n");
//Console.WriteLine(String.Join("\n", wares.Select(p => "[" + wares.IndexOf(p) + "] " + p.Id + "\t|\t" + p.Name)));
//Console.WriteLine($"Wybierz towar (0-{wares.Count - 1})/Dodaj (n):");
//var polecenie = Console.ReadLine();
//if (polecenie.ToLower().Trim() == "n")
//{
    
//}
//var wareIndex = int.Parse(Console.ReadLine());
//var stocks = session.GetStocks(wares[wareIndex]);
//Console.WriteLine(String.Join("\n", stocks.Select(p => p.Id + "\t|\t" + p.Ware + "\t|\t" + p.Quantity)));
