using UnityEngine;

public class CarWashGameManager : MonoBehaviour
{
    private static CarWashGameManager instance;
    public static CarWashGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("CarWashGameManager instance is null! Make sure there is a CarWashGameManager in the scene.");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Multiple CarWashGameManager instances found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("CarWashGameManager initialized successfully.");
    }

    // Add any car wash-specific logic here
    public void OnCarWashed()
    {
        Debug.Log("Car washed successfully!");
    }

    public void UpdateStats(int stockChange, int satisfactionChange, int profitChange, int moraleChange)
{
    CarWashUIManager.Instance.UpdateStock(stockChange);
    CarWashUIManager.Instance.UpdateSatisfaction(satisfactionChange);
    CarWashUIManager.Instance.UpdateProfit(profitChange);
    CarWashUIManager.Instance.UpdateMorale(moraleChange);
}

public void NextDay()
{
    CarWashUIManager.Instance.UpdateDay();
}
}