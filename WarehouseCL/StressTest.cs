using CassandraWarehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CassandraWarehouse.Models;
using System.Diagnostics;

namespace WarehouseCL
{
    public class StressTest
    {
        private BackendSession session;

        public StressTest(BackendSession session)
        {
            this.session = session;
        }

        public void Run()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i< 1000; i++)
            {
                session.AddWare("ST " + i.ToString());
            }
            sw.Stop();

            //Console.WriteLine($"Dodanie 100 towarów [ms]: {sw.ElapsedMilliseconds}");
            Console.Write(sw.ElapsedMilliseconds + ";");
            sw.Restart();
            var wares = session.GetWares();
            sw.Stop();
            //Console.WriteLine($"Pobranie towarów [ms]: {sw.ElapsedMilliseconds}");
            Console.Write(sw.ElapsedMilliseconds + ";");
            Random rand = new Random();
            sw.Restart();
            for (int i = 0; i< 1000; i++)
            {
                Dictionary<Ware, (long, double)> receivePositions = new Dictionary<Ware, (long, double)>();
                for (int j = 0; j < rand.Next(1, 5); j++)
                {
                    var ware = wares[rand.Next(0, wares.Count - 1)];
                    if (receivePositions.ContainsKey(ware)) continue;
                    receivePositions.Add(ware, (rand.Next(1, 20), (double)rand.Next(1, 50)));
                }
                session.AddReceiving("PZ" + i.ToString(), DateTime.Today, "Test klient " + i.ToString(), receivePositions);
            }
            sw.Stop();
            //Console.WriteLine($"Dodawanie 1000 przyjęć [ms]: {sw.ElapsedMilliseconds}");
            Console.Write(sw.ElapsedMilliseconds + ";");
            sw.Restart();
            var stocks = session.GetStocks();
            sw.Stop();
            //Console.WriteLine($"Pobranie zasobów [ms]: {sw.ElapsedMilliseconds}");
            Console.Write(sw.ElapsedMilliseconds + ";");

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                Dictionary<Stock, (long, double)> releasePositions = new Dictionary<Stock, (long, double)>();
                for (int j = 0; j < rand.Next(1, 5); j++)
                {
                    var stock = stocks[rand.Next(0, stocks.Count - 1)];
                    if(releasePositions.ContainsKey(stock)) continue;
                    releasePositions.Add(stock, (rand.Next(1, 20), (double)rand.Next(1, 50)));
                }
                session.AddRelease("WZ" + i.ToString(), DateTime.Today, "Test klient " + i.ToString(), releasePositions);
            }
            sw.Stop();
            //Console.WriteLine($"Dodawanie 1000 wydań [ms]: {sw.ElapsedMilliseconds}");
            Console.Write(sw.ElapsedMilliseconds + ";");
            Console.WriteLine();
        }


    }
}
