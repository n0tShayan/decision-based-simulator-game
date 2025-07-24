using UnityEngine;
using UnityEngine.UI;

public class CarWashUIManager : MonoBehaviour
{
    [SerializeField] private Text satisfactionText;
    [SerializeField] private Text stockText;
    [SerializeField] private Text moraleText;
    [SerializeField] private Text profitText;
    [SerializeField] private Text dayText;

    [SerializeField] private GameObject decisionCardPrefab; // Prefab for the decision card
    [SerializeField] private Transform decisionCardParent; // Parent transform for the decision card UI

    private int satisfaction = 50;
    private int stock = 50;
    private int morale = 50;
    private int profit = 0;
    private int currentDay = 1;

    public static CarWashUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateSatisfaction(int value)
    {
        satisfaction += value;
        satisfaction = Mathf.Clamp(satisfaction, 0, 100);
        UpdateUI();
    }

    public void UpdateStock(int value)
    {
        stock += value;
        stock = Mathf.Max(0, stock);
        UpdateUI();
    }

    public void UpdateMorale(int value)
    {
        morale += value;
        morale = Mathf.Clamp(morale, 0, 100);
        UpdateUI();
    }

    public void UpdateProfit(int value)
    {
        profit += value;
        UpdateUI();
    }

    public void UpdateDay()
    {
        currentDay++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        satisfactionText.text = $"Satisfaction: {satisfaction}";
        stockText.text = $"Stock: {stock}";
        moraleText.text = $"Morale: {morale}";
        profitText.text = $"Profit: ${profit}";
        dayText.text = $"Day: {currentDay}";
    }

    public void ShowDecisionCard(CarWashDecision decision)
    {
        // Instantiate the decision card prefab
        GameObject decisionCard = Instantiate(decisionCardPrefab, decisionCardParent);

        // Get the CarWashDecisionCard component and pass the decision data
        carwashdecisioncardUI decisionCardScript = decisionCard.GetComponent<carwashdecisioncardUI>();
        if (decisionCardScript != null)
        {
            decisionCardScript.ShowDecision(decision);
        }
        else
        {
            Debug.LogError("DecisionCard prefab is missing the CarWashDecisionCard script!");
        }
    }
}
