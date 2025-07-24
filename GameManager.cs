using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("GameManager instance is null! Make sure there is a GameManager in the scene.");
            }
            return instance;
        }
    }

    // Game Stats
    public int Stock { get; private set; } = 50;
    public int CustomerSatisfaction { get; private set; } = 50;
    public int Profit { get; private set; } = 50;
    public int StaffMorale { get; private set; } = 50;
    public int CurrentDay { get; private set; } = 1;

    // Game State
    public bool IsGameOver { get; private set; } = false;

    [SerializeField] private GameObject decisionCardPrefab; // Assign in Inspector
    [SerializeField] private float dayDuration = 60f; // Seconds per day
    [SerializeField] private GameObject dollarAnimationPrefab;
    private float dayTimer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Multiple GameManager instances found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager initialized successfully.");
    }

    private void Start()
    {
        try
        {
            DatabaseManager.Instance.StartGameSession(1); // PlayerID 1
            DatabaseManager.Instance.UpdatePlayerDays(1, CurrentDay);
            dayTimer = dayDuration;
            SpawnTestDecision();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start game: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void Update()
    {
        if (IsGameOver) return;

        dayTimer -= Time.deltaTime;
        if (dayTimer <= 0)
        {
            NextDay();
        }
    }

    public void UpdateStats(int stockChange, int satisfactionChange, int profitChange, int moraleChange)
    {
        if (IsGameOver) return;

        try
        {
            Stock = Mathf.Clamp(Stock + stockChange, 0, 100);
            CustomerSatisfaction = Mathf.Clamp(CustomerSatisfaction + satisfactionChange, 0, 100);
            Profit = Mathf.Clamp(Profit + profitChange, 0, 100);
            StaffMorale = Mathf.Clamp(StaffMorale + moraleChange, 0, 100);

            Debug.Log($"Stats Updated - Stock: {Stock}, Satisfaction: {CustomerSatisfaction}, Profit: {Profit}, Morale: {StaffMorale}");
            CheckGameOver();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to update stats: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void CheckGameOver()
    {
        string reason = null;
        if (Stock <= 0) reason = "Stockout";
        else if (CustomerSatisfaction <= 0) reason = "Boycott";
        else if (Profit <= 0) reason = "Bankruptcy";
        else if (StaffMorale <= 0) reason = "Strike";

        if (reason != null)
        {
            GameOver(reason);
        }
    }

    private void GameOver(string reason)
    {
        try
        {
            IsGameOver = true;
            DatabaseManager.Instance.EndGameSession(1, reason, Stock, CustomerSatisfaction, Profit, StaffMorale);
            Debug.Log($"Game Over! Reason: {reason} on Day {CurrentDay}");

            // Stop all customer spawning
            CharacterSpawner spawner = FindFirstObjectByType<CharacterSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
            }
            else
            {
                Debug.LogWarning("CharacterSpawner not found in scene!");
            }

            // Stop all customer movement
            CustomerMovement[] customers = FindObjectsByType<CustomerMovement>(FindObjectsSortMode.None);
            foreach (var customer in customers)
            {
                customer.StopMovement();
            }

            // Show game over screen
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowGameOver(reason);
            }
            else
            {
                Debug.LogError("UIManager not found in scene!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to end game: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void NextDay()
    {
        if (IsGameOver) return;

        try
        {
            CurrentDay++;
            DatabaseManager.Instance.UpdatePlayerDays(1, CurrentDay);
            Debug.Log($"Moving to Day {CurrentDay}");

            // Simulate daily shopping
            int stockReduction = UnityEngine.Random.Range(5, 15);
            UpdateStats(-stockReduction, 0, 0, 0);

            // Check low stock and spawn decision
            var lowStockItems = DatabaseManager.Instance.GetLowStockItems();
            foreach (var item in lowStockItems)
            {
                SpawnDecisionCard(item.ItemID, item.Name);
            }

            dayTimer = dayDuration;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to advance to next day: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void ReportLowStock(int itemID)
    {
        if (IsGameOver) return;

        try
        {
            var item = DatabaseManager.Instance.GetItemByID(itemID);
            if (item == null)
            {
                Debug.LogWarning($"Item {itemID} not found for restock");
                return;
            }

            int quantity = 20;
            DatabaseManager.Instance.UpdateItemStock(itemID, -quantity); // Negative to add stock
            DatabaseManager.Instance.CreateSupplierOrder(itemID, quantity, quantity * item.UnitPrice, "Pending");
            UpdateStats(quantity, 0, -5, 0); // Restock adds stock, costs profit
            Debug.Log($"Restocked item {itemID} ({item.Name}) with {quantity} units, ordered from supplier");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to report low stock for item {itemID}: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public int GetItemStock(int itemID)
    {
        try
        {
            int stock = DatabaseManager.Instance.GetItemStock(itemID);
            Debug.Log($"Retrieved stock for item {itemID}: {stock}");
            return stock;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get stock for item {itemID}: {e.Message}\nStackTrace: {e.StackTrace}");
            return 0;
        }
    }

    private void SpawnTestDecision()
    {
        try
        {
            if (decisionCardPrefab == null)
            {
                Debug.LogError("DecisionCardPrefab not assigned in GameManager!");
                return;
            }
            GameObject card = Instantiate(decisionCardPrefab);
            DecisionCard decisionCard = card.GetComponent<DecisionCard>();
            if (decisionCard == null)
            {
                Debug.LogError("DecisionCard component not found on prefab!");
                Destroy(card);
                return;
            }
            decisionCard.Setup("Restock Milk?", 1, 20, 0, -5, 0);
            Debug.Log("Spawned test decision card");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to spawn test decision card: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }




    public void PlayDollarAnimation(Vector3 position)
    {
        if (dollarAnimationPrefab == null)
        {
            Debug.LogWarning("Dollar animation prefab not assigned!");
            return;
        }

        Vector3 spawnPos = new Vector3(position.x, position.y, -2f); // Z in front of customer
        GameObject clone = Instantiate(dollarAnimationPrefab, spawnPos, Quaternion.identity);
        Destroy(clone, 1f);
    }


    private void SpawnDecisionCard(int itemID, string itemName)
    {
        try
        {
            if (decisionCardPrefab == null)
            {
                Debug.LogError("DecisionCardPrefab not assigned in GameManager!");
                return;
            }
            GameObject card = Instantiate(decisionCardPrefab);
            DecisionCard decisionCard = card.GetComponent<DecisionCard>();
            if (decisionCard == null)
            {
                Debug.LogError("DecisionCard component not found on prefab!");
                Destroy(card);
                return;
            }
            string description = $"Restock {itemName}? (Cost: 5 Profit)";
            decisionCard.Setup(description, itemID, 20, 0, -5, 0);
            Debug.Log($"Spawned decision card for low stock item: {itemName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to spawn decision card for item {itemID}: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }
}