using UnityEngine;
using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;
using System;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager instance;
    public static DatabaseManager Instance { get { return instance; } }

    private SQLiteConnection dbConnection;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            string dbPath = Path.Combine(Application.dataPath, "SupermartDB.db");
            Debug.Log("Database will be created at: " + dbPath);

            dbConnection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            CreateTables();
            Debug.Log("Database initialized successfully at: " + dbPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize database: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void CreateTables()
    {
        try
        {
            dbConnection.CreateTable<Player>();
            dbConnection.CreateTable<Item>();
            dbConnection.CreateTable<Customer>();
            dbConnection.CreateTable<Transaction>();
            dbConnection.CreateTable<DecisionLog>();
            dbConnection.CreateTable<GameSession>();
            dbConnection.CreateTable<SupplierOrder>();

            // Insert initial data
            if (dbConnection.Table<Player>().Count() == 0)
            {
                dbConnection.Insert(new Player { Username = "You", DaysSurvived = 0 });
                dbConnection.Commit();
                Debug.Log("Initial player added");
            }

            if (dbConnection.Table<Item>().Count() == 0)
            {
                dbConnection.InsertAll(new[]
                {
                    new Item { Name = "Milk", StockLevel = 50, SalesCount = 0, UnitPrice = 2.5f, XCoord = 2, YCoord = 2 },
                    new Item { Name = "Bread", StockLevel = 50, SalesCount = 0, UnitPrice = 1.5f, XCoord = 3, YCoord = 2 },
                    new Item { Name = "Apple", StockLevel = 50, SalesCount = 0, UnitPrice = 0.5f, XCoord = 4, YCoord = 2 },
                    new Item { Name = "Eggs", StockLevel = 50, SalesCount = 0, UnitPrice = 3.0f, XCoord = 5, YCoord = 2 },
                    new Item { Name = "Cheese", StockLevel = 50, SalesCount = 0, UnitPrice = 4.0f, XCoord = 6, YCoord = 2 }
                });
                dbConnection.Commit();
                Debug.Log("Initial items added");
            }

            // Removed initial customers to allow dynamic additions
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create tables: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void UpdateItemStock(int itemID, int changeAmount)
    {
        try
        {
            if (itemID <= 0) throw new ArgumentException($"Invalid itemID: {itemID}");
            var item = dbConnection.Table<Item>().Where(i => i.ItemID == itemID).FirstOrDefault();
            if (item == null)
            {
                Debug.LogWarning($"Item {itemID} not found for stock update");
                return;
            }

            item.StockLevel -= changeAmount;
            item.SalesCount += changeAmount;
            dbConnection.Update(item);
            dbConnection.Commit();
            Debug.Log($"Updated item {itemID} stock to {item.StockLevel}, sales to {item.SalesCount}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to update item stock (itemID: {itemID}): {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public int GetItemStock(int itemID)
    {
        try
        {
            if (itemID <= 0) throw new ArgumentException($"Invalid itemID: {itemID}");
            var item = dbConnection.Table<Item>().Where(i => i.ItemID == itemID).FirstOrDefault();
            int stock = item?.StockLevel ?? 0;
            Debug.Log($"Retrieved stock for item {itemID}: {stock}");
            return stock;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get item stock (itemID: {itemID}): {e.Message}\nStackTrace: {e.StackTrace}");
            return 0;
        }
    }

    public Item GetItemByID(int itemID)
    {
        try
        {
            if (itemID <= 0) throw new ArgumentException($"Invalid itemID: {itemID}");
            var item = dbConnection.Table<Item>().Where(i => i.ItemID == itemID).FirstOrDefault();
            if (item == null)
            {
                Debug.LogWarning($"Item {itemID} not found");
            }
            Debug.Log($"Retrieved item {itemID}: {item?.Name}");
            return item;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get item {itemID}: {e.Message}\nStackTrace: {e.StackTrace}");
            return null;
        }
    }

    public int AddNewCustomer()
    {
        try
        {
            string[] names = new[] { "Emma Davis", "Liam Wilson", "Olivia Brown", "Noah Taylor", "Ava Clark", "Sophia Lee", "James White", "Mia Harris" };
            string randomName = names[UnityEngine.Random.Range(0, names.Length)];
            int loyaltyPoints = UnityEngine.Random.Range(10, 100);

            var customer = new Customer
            {
                Name = randomName,
                LoyaltyPoints = loyaltyPoints
            };
            dbConnection.Insert(customer);
            dbConnection.Commit();

            // Get the inserted CustomerID
            var insertedCustomer = dbConnection.Table<Customer>().Where(c => c.Name == randomName && c.LoyaltyPoints == loyaltyPoints).FirstOrDefault();
            Debug.Log($"Added new customer: {randomName}, LoyaltyPoints: {loyaltyPoints}, CustomerID: {insertedCustomer.CustomerID}");
            return insertedCustomer.CustomerID;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add new customer: {e.Message}\nStackTrace: {e.StackTrace}");
            return 0;
        }
    }

    public void RecordTransaction(int customerID, int itemID, int quantity, float totalCost, int satisfactionChange)
    {
        try
        {
            if (customerID <= 0 || itemID <= 0 || quantity <= 0)
                throw new ArgumentException($"Invalid transaction data: customerID={customerID}, itemID={itemID}, quantity={quantity}");
            var transaction = new Transaction
            {
                CustomerID = customerID,
                ItemID = itemID,
                Quantity = quantity,
                TotalCost = totalCost,
                PurchaseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                SatisfactionChange = satisfactionChange
            };
            dbConnection.Insert(transaction);
            dbConnection.Commit();
            Debug.Log($"Recorded transaction: customer {customerID}, item {itemID}, quantity {quantity}, cost {totalCost}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to record transaction (customerID: {customerID}, itemID: {itemID}): {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void RecordDecision(int playerID, string description, string choiceMade, int? itemID, int stockChange, int satisfactionChange, int profitChange, int moraleChange)
    {
        try
        {
            if (playerID <= 0 || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(choiceMade))
                throw new ArgumentException($"Invalid decision data: playerID={playerID}, description={description}, choice={choiceMade}");
            var decision = new DecisionLog
            {
                PlayerID = playerID,
                Description = description,
                ChoiceMade = choiceMade,
                ItemID = itemID,
                StockChange = stockChange,
                SatisfactionChange = satisfactionChange,
                ProfitChange = profitChange,
                MoraleChange = moraleChange,
                DecisionTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            dbConnection.Insert(decision);
            dbConnection.Commit();
            Debug.Log($"Recorded decision: {description}, choice: {choiceMade}, itemID: {itemID}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to record decision (playerID: {playerID}): {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void StartGameSession(int playerID)
    {
        try
        {
            if (playerID <= 0) throw new ArgumentException($"Invalid playerID: {playerID}");
            var session = new GameSession
            {
                PlayerID = playerID,
                StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                FinalStock = 50,
                FinalSatisfaction = 50,
                FinalProfit = 50,
                FinalMorale = 50
            };
            dbConnection.Insert(session);
            dbConnection.Commit();
            Debug.Log($"Started game session for player {playerID}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start game session (playerID: {playerID}): {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void EndGameSession(int sessionID, string endReason, int finalStock, int finalSatisfaction, int finalProfit, int finalMorale)
    {
        try
        {
            if (sessionID <= 0) throw new ArgumentException($"Invalid sessionID: {sessionID}");
            var session = dbConnection.Table<GameSession>().Where(s => s.SessionID == sessionID).FirstOrDefault();
            if (session == null)
            {
                Debug.LogWarning($"Session {sessionID} not found for update");
                return;
            }
            session.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            session.EndReason = endReason;
            session.FinalStock = finalStock;
            session.FinalSatisfaction = finalSatisfaction;
            session.FinalProfit = finalProfit;
            session.FinalMorale = finalMorale;
            dbConnection.Update(session);
            dbConnection.Commit();
            Debug.Log($"Ended game session {sessionID}: {endReason}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to end game session (sessionID: {sessionID}): {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void CreateSupplierOrder(int itemID, int quantity, float orderCost, string status)
    {
        try
        {
            if (itemID <= 0 || quantity <= 0 || string.IsNullOrEmpty(status))
                throw new ArgumentException($"Invalid order data: itemID={itemID}, quantity={quantity}, status={status}");
            var order = new SupplierOrder
            {
                ItemID = itemID,
                Quantity = quantity,
                OrderCost = orderCost,
                OrderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = status
            };
            dbConnection.Insert(order);
            dbConnection.Commit();
            Debug.Log($"Created supplier order: item {itemID}, quantity {quantity}, status {status}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create supplier order (itemID: {itemID}): {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public List<Item> GetLowStockItems(int threshold = 10)
    {
        try
        {
            var items = dbConnection.Table<Item>().Where(i => i.StockLevel < threshold).ToList();
            Debug.Log($"Retrieved {items.Count} low stock items (threshold: {threshold})");
            return items;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get low stock items: {e.Message}\nStackTrace: {e.StackTrace}");
            return new List<Item>();
        }
    }

    public void UpdatePlayerDays(int playerID, int days)
    {
        try
        {
            if (playerID <= 0) throw new ArgumentException($"Invalid playerID: {playerID}");
            var player = dbConnection.Table<Player>().Where(p => p.PlayerID == playerID).FirstOrDefault();
            if (player == null)
            {
                Debug.LogWarning($"Player {playerID} not found for days update");
                return;
            }
            player.DaysSurvived = days;
            dbConnection.Update(player);
            dbConnection.Commit();
            Debug.Log($"Updated player {playerID} days to {days}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to update player days (playerID: {playerID}): {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void OnDestroy()
    {
        if (dbConnection != null)
        {
            dbConnection.Close();
            Debug.Log("Database connection closed");
        }
    }
}

[Table("Player")]
public class Player
{
    [PrimaryKey, AutoIncrement]
    public int PlayerID { get; set; }
    public string Username { get; set; }
    public int DaysSurvived { get; set; }
}

[Table("Item")]
public class Item
{
    [PrimaryKey, AutoIncrement]
    public int ItemID { get; set; }
    public string Name { get; set; }
    public int StockLevel { get; set; }
    public int SalesCount { get; set; }
    public float UnitPrice { get; set; }
    public float XCoord { get; set; }
    public float YCoord { get; set; }
}

[Table("Customer")]
public class Customer
{
    [PrimaryKey, AutoIncrement]
    public int CustomerID { get; set; }
    public string Name { get; set; }
    public int LoyaltyPoints { get; set; }
}

[Table("Transaction")]
public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int TransactionID { get; set; }
    public int CustomerID { get; set; }
    public int ItemID { get; set; }
    public int Quantity { get; set; }
    public float TotalCost { get; set; }
    public string PurchaseTime { get; set; }
    public int SatisfactionChange { get; set; }
}

[Table("DecisionLog")]
public class DecisionLog
{
    [PrimaryKey, AutoIncrement]
    public int DecisionID { get; set; }
    public int PlayerID { get; set; }
    public string Description { get; set; }
    public string ChoiceMade { get; set; }
    public int? ItemID { get; set; }
    public int StockChange { get; set; }
    public int SatisfactionChange { get; set; }
    public int ProfitChange { get; set; }
    public int MoraleChange { get; set; }
    public string DecisionTime { get; set; }
}

[Table("GameSession")]
public class GameSession
{
    [PrimaryKey, AutoIncrement]
    public int SessionID { get; set; }
    public int PlayerID { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string EndReason { get; set; }
    public int FinalStock { get; set; }
    public int FinalSatisfaction { get; set; }
    public int FinalProfit { get; set; }
    public int FinalMorale { get; set; }
}

[Table("SupplierOrder")]
public class SupplierOrder
{
    [PrimaryKey, AutoIncrement]
    public int OrderID { get; set; }
    public int ItemID { get; set; }
    public int Quantity { get; set; }
    public float OrderCost { get; set; }
    public string OrderTime { get; set; }
    public string Status { get; set; }
}