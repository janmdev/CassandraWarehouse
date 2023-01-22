// See https://aka.ms/new-console-template for more information
using Cassandra;
using CassandraWarehouse;
using WarehouseCL;
using System.Configuration;


Startup startup = new Startup();
BackendSession session = new BackendSession(startup.CassSettings.Host, startup.CassSettings.KeySpace);

var stressTest = new StressTest(session);
stressTest.Run();

Menu menu = new Menu(session);
menu.Run();