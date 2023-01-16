﻿using Cassandra;
using CassandraWarehouse.Models;
using System.Text.Json;

namespace CassandraWarehouse;
public class BackendSession
{
    private ISession session;
    private PreparedStatement getStocksByWareStatement;
    private PreparedStatement getWaresStatement;
    private PreparedStatement incrStockStatement;
    private PreparedStatement decrStockStatement;
    private PreparedStatement deleteStockStatement;
    private PreparedStatement deleteWareStatement;
    private PreparedStatement insertWareStatement;
    private PreparedStatement insertReceivingStatement;
    private PreparedStatement getReceivingsStatement;

    public BackendSession(string host, string keySpace)
    {
        var cluster = Cluster.Builder()
                     .AddContactPoints(host)
                     .Build();
        session = cluster.Connect(keySpace);
        getStocksByWareStatement = session.Prepare("SELECT * FROM stocks WHERE ware=?");
        getWaresStatement = session.Prepare("SELECT * FROM wares");
        deleteWareStatement = session.Prepare("DELETE FROM wares where id=?");
        deleteStockStatement = session.Prepare("DELETE FROM stocks where receiving=? and ware=?");
        insertWareStatement = session.Prepare("INSERT INTO wares (id, name) VALUES (?,?)");
        //insertStockStatement = session.Prepare("INSERT INTO stocks (id, ware, quantity) VALUES (?,?,?)");
        incrStockStatement = session.Prepare("UPDATE stocks  SET quantity = quantity + ? where receiving = ? and ware = ?");
        decrStockStatement = session.Prepare("UPDATE stocks  SET quantity = quantity - ? where receiving = ? and ware = ?");
        insertReceivingStatement = session.Prepare("INSERT INTO receivings (id, number, date, client, positions) VALUES (?,?,?,?,?)");
        getReceivingsStatement = session.Prepare("SELECT * FROM receivings");
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

    public List<Stock> GetStocks(Guid? wareId)
    {
        var statement = getStocksByWareStatement.Bind(wareId);
        RowSet rows = session.Execute(statement);
        List<Stock> stocks = rows.Select(p => new Stock((Guid?)p[0], (Guid?)p[1], (long?)p[2])).ToList();
        return stocks;
    }

    public void AddReceiving(string? number, DateTime date, string? client, Dictionary<Ware, long> positions)
    {
        Guid receivingGuid = Guid.NewGuid();
        List<Stock> stocks = positions.Select(p => new Stock(receivingGuid, p.Key.Id, p.Value)).ToList();
        var positionsJson = JsonSerializer.Serialize(stocks);
        var recvStatement = insertReceivingStatement.Bind(receivingGuid, number, date, client, positionsJson);
        session.Execute(recvStatement);
        stocks.ForEach(p => addStock(p));
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

    public void AddWare(string name)
    {
        var statement = insertWareStatement.Bind(Guid.NewGuid(), name);
        session.Execute(statement);
    }

    public void DeleteStock(Stock stock)
    {
        DeleteStock(stock.Receiving, stock.Ware);
    }

    public void DeleteStock(Guid? stockId, Guid? wareId)
    {
        var statement = deleteStockStatement.Bind(stockId, wareId);
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
        List<Receiving> receivings = rows.Select(p => new Receiving((Guid?)p[1], (string?)p[3], (DateTimeOffset)p[0], (string?)p[2], (string?)p[4])).ToList();
        return receivings;
    }
}