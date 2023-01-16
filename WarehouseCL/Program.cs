// See https://aka.ms/new-console-template for more information
using Cassandra;
using CassandraWarehouse;
using WarehouseCL;

Menu menu = new Menu(new BackendSession("localhost", "warehouse"));

menu.Run();