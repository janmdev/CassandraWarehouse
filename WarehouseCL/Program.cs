// See https://aka.ms/new-console-template for more information
using Cassandra;
using CassandraWarehouse;
using WarehouseCL;
using System.Configuration;


Startup startup = new Startup();

Menu menu = new Menu(new BackendSession(startup.CassSettings.Host, startup.CassSettings.KeySpace));

menu.Run();