using Cassandra;
using CassandraWarehouse.Models;
using System.Reflection;
using System.Text.Json;

namespace CassandraWarehouse;
public class BackendSession
{
    private ISession session;
    private PreparedStatement getStocksByWareStatement;
    private PreparedStatement getStocksStatement;
    private PreparedStatement getWaresStatement;
    private PreparedStatement incrStockStatement;
    private PreparedStatement decrStockStatement;
    private PreparedStatement deleteWareStatement;
    private PreparedStatement insertWareStatement;
    private PreparedStatement insertReceivingStatement;
    private PreparedStatement getReceivingsStatement;
    private PreparedStatement getReleasesStatement;
    private PreparedStatement insertReleaseStatement;
    private readonly ConsistencyLevel warehouseMoveConstLevel = ConsistencyLevel.One;

    public BackendSession(string host, string keySpace)
    {
        var cluster = Cluster.Builder()
                     .AddContactPoints(host).WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.One))
                     .Build();
        session = cluster.Connect(keySpace);
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "CassandraWarehouse.warehouse table def.sql";
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            string result = reader.ReadToEnd();
            var queries = result.Split(';').Select(p => p.Trim().Replace("\r\n","")).Where(p => !String.IsNullOrEmpty(p));
            foreach(var query in queries) session.Execute(query);
        }
        getStocksByWareStatement = session.Prepare("SELECT * FROM stocks WHERE ware=?");
        getStocksStatement = session.Prepare("SELECT * FROM stocks");
        getWaresStatement = session.Prepare("SELECT * FROM wares");
        deleteWareStatement = session.Prepare("DELETE FROM wares where id=?");
        insertWareStatement = session.Prepare("INSERT INTO wares (id, name) VALUES (?,?)"); 
        incrStockStatement = session.Prepare("UPDATE stocks  SET quantity = quantity + ? where receiving = ? and ware = ?").SetConsistencyLevel(warehouseMoveConstLevel);
        decrStockStatement = session.Prepare("UPDATE stocks  SET quantity = quantity - ? where receiving = ? and ware = ?").SetConsistencyLevel(warehouseMoveConstLevel);
        insertReceivingStatement = session.Prepare("INSERT INTO receivings (id, number, date, client, positions) VALUES (?,?,?,?,?)").SetConsistencyLevel(warehouseMoveConstLevel);
        insertReleaseStatement = session.Prepare("INSERT INTO releases (id, number, date, client, positions) VALUES (?,?,?,?,?)").SetConsistencyLevel(warehouseMoveConstLevel);
        getReceivingsStatement = session.Prepare("SELECT * FROM receivings");
        getReleasesStatement = session.Prepare("SELECT * FROM releases");
    }

    public List<Ware> GetWares()
    {
        var statement = getWaresStatement.Bind();
        RowSet rows = session.Execute(statement);
        List<Ware> wares = rows.Select(p => new Ware((Guid?)p[0], (string?)p[1])).ToList();
        return wares;
    }

    public List<Stock> GetStocks(Ware ware)
    {
        return GetStocks(ware.Id);
    }

    public List<Stock> GetStocks()
    {
        var statement = getStocksStatement.Bind();
        RowSet rows = session.Execute(statement);
        List<Stock> stocks = rows.Select(p => new Stock((Guid?)p[1], (Guid?)p[0], (long?)p[2])).ToList();
        return stocks;
    }

    public List<Stock> GetStocks(Guid? wareId)
    {
        var statement = getStocksByWareStatement.Bind(wareId);
        RowSet rows = session.Execute(statement);
        List<Stock> stocks = rows.Select(p => new Stock((Guid?)p[1], (Guid?)p[0], (long?)p[2])).ToList();
        return stocks;
    }

    public void AddReceiving(string? number, DateTime date, string? client, Dictionary<Ware, (long, double)> positions)
    {
        Guid receivingGuid = Guid.NewGuid();
        List<Stock> stocks = positions.Select(p => new Stock(receivingGuid, p.Key.Id, p.Value.Item1, p.Value.Item2)).ToList();
        var positionsJson = JsonSerializer.Serialize(stocks);
        var recvStatement = insertReceivingStatement.Bind(receivingGuid, number, date.ToLocalDate(), client, positionsJson);
        session.Execute(recvStatement);
        stocks.ForEach(p => addStock(p));
    }

    public void AddRelease(string? number, DateTime date, string? client, Dictionary<Stock, (long, double)> positions)
    {
        Guid releaseGuid = Guid.NewGuid();
        List<Stock> stocks = positions.Select(p => new Stock(p.Key.Receiving, p.Key.Ware, p.Value.Item1, p.Value.Item2)).ToList();
        var positionsJson = JsonSerializer.Serialize(stocks);
        var relsStatement = insertReleaseStatement.Bind(releaseGuid, number, date.ToLocalDate(), client, positionsJson);
        session.Execute(relsStatement);
        stocks.ForEach(p => reduceStock(p));
    }

    private void addStock(Stock stock)
    {
        addStock(stock.Receiving, stock.Ware, stock.Quantity);
    }

    private void addStock(Guid? receiving, Ware ware, long quantity)
    {
        addStock(receiving, ware.Id, quantity);
    }

    private void addStock(Guid? receiving, Guid? wareId, long? quantity)
    {
        var statement = incrStockStatement.Bind(quantity, receiving, wareId);
        session.Execute(statement);
    }

    private void reduceStock(Stock stock)
    {
        var statement = decrStockStatement.Bind(stock.Quantity, stock.Receiving, stock.Ware);
        session.Execute(statement);
    }

    public void AddWare(string name)
    {
        var statement = insertWareStatement.Bind(Guid.NewGuid(), name);
        session.Execute(statement);
    }

    public void DeleteWare(Ware ware)
    {
        DeleteWare(ware.Id);
    }

    public void DeleteWare(Guid? wareId)
    {
        var statement = deleteWareStatement.Bind(wareId);
        session.Execute(statement);
    }

    public List<Receiving> GetReceivings()
    {
        var statement = getReceivingsStatement.Bind();
        RowSet rows = session.Execute(statement);
        List<Receiving> receivings = rows.Select(p => new Receiving((Guid?)p[1], (string?)p[3], (LocalDate)p[0], (string?)p[2], (string?)p[4])).ToList();
        return receivings;
    }

    public List<Release> GetReleases()
    {
        var statement = getReleasesStatement.Bind();
        RowSet rows = session.Execute(statement);
        List<Release> receivings = rows.Select(p => new Release((Guid?)p[1], (string?)p[3], (LocalDate)p[0], (string?)p[2], (string?)p[4])).ToList();
        return receivings;
    }
}
