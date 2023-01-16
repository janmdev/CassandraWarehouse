using Cassandra;
using CassandraWarehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;
using CassandraWarehouse.Models;

namespace WarehouseCL
{
    public class Menu
    {
        private BackendSession session;
        public Menu(BackendSession session)
        {
            this.session = session;
        }

        public void ShowMenu()
        {
            Console.WriteLine("\n" +
                "1 - wyświetl towary\n" +
                "2 - wyświetl zasoby\n" +
                "3 - wyświetl przyjęcia\n" +
                "4 - dodaj towar\n" +
                "5 - dodaj przyjęcie\n" +
                "6 - dodaj wydanie\n" +
                "7 - usuń przyjęcie\n" +
                "8 - usuń wydania\n" +
                "9 - usuń kontrahenta\n" +
                "q - wyjdź");
        }

        public void Run()
        {
            string command = "";
            while (command != "q")
            {
                ShowMenu();
                command = Console.ReadLine().ToLower().Trim();
                switch (command)
                {
                    case "1":
                        showWares();
                        break;
                    case "2":
                        showStocks();
                        break;
                    case "3":
                        showReceivings();
                        break;
                    case "4":
                        addWare();
                        break;
                    case "5":
                        addReceiving();
                        break;
                }
            }
        }

        private void showReceivings()
        {
            var receivings = session.GetReceivings();
            ConsoleTable table = new ConsoleTable("id", "numer", "data", "klient", "pozycje");
            receivings.ForEach(p => table.AddRow(p.Id, p.Number, p.Date.ToString("yyyy-MM-dd"), p.Client, p.Positions));
            table.Write(Format.Minimal);
        }

        private void addReceiving()
        {
            Console.WriteLine("Podaj numer przyjęcia:");
            string receivingNumber = Console.ReadLine();
            Console.WriteLine("Podaj datę przyjęcia (yyyy-MM-dd) [domyślnie - dzisiaj]:");
            string receivingDateS = Console.ReadLine();
            DateTime receivingDate = DateTime.Now;
            if (!String.IsNullOrEmpty(receivingDateS) && !DateTime.TryParse(receivingDateS, out receivingDate))
            {
                Console.WriteLine("Błędny format daty");
                return;
            }
            Console.WriteLine("Podaj kontrahenta:");
            string receivingClient = Console.ReadLine();
            var wares = session.GetWares();
            string wybor = "";
            var table = new ConsoleTable("no.", "uuid", "nazwa");
            wares.ForEach(w => table.AddRow("[" + wares.IndexOf(w) + "] ", w.Id, w.Name));
            table.Write(Format.Minimal);
            Dictionary<Ware, long> positions = new Dictionary<Ware, long>();
            while(wybor != "q")
            {
                Console.WriteLine($"Wybierz towar: (0-{wares.Count - 1}) [q - koniec]:");
                wybor = Console.ReadLine().ToLower().Trim();
                if (int.TryParse(wybor, out int wareInd))
                {
                    if (wareInd > wares.Count - 1)
                    {
                        Console.WriteLine("Błędny numer towaru");
                        return;
                    }
                    Ware ware = wares[wareInd];
                    Console.WriteLine("Podaj ilość towaru");
                    if (long.TryParse(Console.ReadLine(), out long quantity))
                    {
                        if (quantity <= 0)
                        {
                            Console.WriteLine("Błędna ilość towaru");
                            return;
                        }
                        if (!positions.ContainsKey(ware)) positions.Add(ware, quantity);
                        else positions[ware] = quantity;
                    }
                    else
                    {
                        Console.WriteLine("Błędna ilość towaru");
                        return;
                    }
                }
                else if (wybor.ToLower().Trim() != "q")
                {
                    Console.WriteLine("Błędny numer towaru");
                    return;
                }
            }
            session.AddReceiving(receivingNumber, receivingDate, receivingClient ,positions);
        }

        private void addWare()
        {
            Console.WriteLine("Podaj nazwę: ");
            string name = Console.ReadLine();
            session.AddWare(name);
        }

        private void showWares()
        {
            var wares = session.GetWares();
            var table = new ConsoleTable("no.", "uuid", "nazwa");
            wares.ForEach(w => table.AddRow("[" + wares.IndexOf(w) + "] ", w.Id, w.Name));
            table.Write(Format.Minimal);
        }

        private void showStocks()
        {
            var wares = session.GetWares();
            var waresTable = new ConsoleTable("no.", "uuid", "nazwa");
            wares.ForEach(w => waresTable.AddRow("[" + wares.IndexOf(w) + "] ", w.Id, w.Name));
            waresTable.Write(Format.Minimal);
            Console.WriteLine();
            Console.WriteLine($"Wybierz towar (0-{wares.Count - 1}):");
            if (int.TryParse(Console.ReadLine(), out var wareIndex))
            {
                var stocks = session.GetStocks(wares[wareIndex]);
                var stocksTable = new ConsoleTable("uuid", "nazwa", "ilosc");
                stocks.ForEach(p => stocksTable.AddRow(p.Receiving, p.Ware,p.Quantity));
                stocksTable.Write(Format.Minimal);
            }
            else Console.WriteLine("Błędny format polecenia");
           
        }
        
    }

    
}
