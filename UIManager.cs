using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI stockText;
    [SerializeField] private TextMeshProUGUI satisfactionText;
    [SerializeField] private TextMeshProUGUI profitText;
    [SerializeField] private TextMeshProUGUI moraleText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button reportButton;
    [SerializeField] private GameObject decisionCardPrefab;

    private void Start()
    {
        // Check if all UI elements are assigned
        if (stockText == null) Debug.LogError("Stock Text is not assigned in UIManager!");
        if (satisfactionText == null) Debug.LogError("Satisfaction Text is not assigned in UIManager!");
        if (profitText == null) Debug.LogError("Profit Text is not assigned in UIManager!");
        if (moraleText == null) Debug.LogError("Morale Text is not assigned in UIManager!");
        if (dayText == null) Debug.LogError("Day Text is not assigned in UIManager!");
        if (gameOverPanel == null) Debug.LogError("Game Over Panel is not assigned in UIManager!");
        if (gameOverText == null) Debug.LogError("Game Over Text is not assigned in UIManager!");
        if (reportButton == null) Debug.LogError("Report Button is not assigned in UIManager!");

        // Add listener to report button if assigned
        if (reportButton != null)
        {
            reportButton.onClick.AddListener(OnReportButtonClick);
        }

        // Initial UI update
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Check if GameManager instance exists
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is null!");
            return;
        }

        // Update all UI texts
        if (stockText != null)
            stockText.text = $"Stock: {GameManager.Instance.Stock}";
        
        if (satisfactionText != null)
            satisfactionText.text = $"Satisfaction: {GameManager.Instance.CustomerSatisfaction}";
        
        if (profitText != null)
            profitText.text = $"Profit: {GameManager.Instance.Profit}";
        
        if (moraleText != null)
            moraleText.text = $"Morale: {GameManager.Instance.StaffMorale}";
        
        if (dayText != null)
            dayText.text = $"Day: {GameManager.Instance.CurrentDay}";
    }

    private void OnReportButtonClick()
    {
        // Check for low stock items
        for (int itemID = 1; itemID <= 3; itemID++)
        {
            int stock = GameManager.Instance.GetItemStock(itemID);
            if (stock < 10)
            {
                GameManager.Instance.ReportLowStock(itemID);
                Debug.Log($"Reported low stock for item {itemID}");
            }
        }
    }

    public void ShowGameOver(string reason)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
            {
                gameOverText.text = $"Game Over!\nReason: {reason}\nDays Survived: {GameManager.Instance.CurrentDay}";
            }
        }
    }

    public void ShowDecisionCard(Decision decision)
    {
        GameObject card = Instantiate(decisionCardPrefab, transform);
        DecisionCard decisionCard = card.GetComponent<DecisionCard>();
        decisionCard.ShowDecision(decision);
    }
} 