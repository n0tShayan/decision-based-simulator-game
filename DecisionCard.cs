using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DecisionCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private TextMeshProUGUI rightButtonText;
    [SerializeField] private float displayDuration = 10f; // How long the card stays before auto-choosing

    private Decision currentDecision;
    private float timeRemaining;
    private bool isActive = false;
    private int? itemID; // Track itemID for database logging
    private int playerID = 1; // Default to "You"

    private void Start()
    {
        try
        {
            // Validate UI references
            if (descriptionText == null || leftButton == null || rightButton == null || leftButtonText == null || rightButtonText == null)
            {
                Debug.LogError("DecisionCard UI references not assigned!");
                Destroy(gameObject);
                return;
            }

            // Add button listeners
            leftButton.onClick.AddListener(OnLeftChoice);
            rightButton.onClick.AddListener(OnRightChoice);
            Debug.Log("DecisionCard initialized with button listeners.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize DecisionCard: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void Update()
    {
        if (isActive)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                // Auto-choose the right option if time runs out
                OnRightChoice();
            }
        }
    }

    public void Setup(string desc, int? itemId, int stock, int satisfaction, int profit, int morale)
    {
        try
        {
            if (string.IsNullOrEmpty(desc))
            {
                Debug.LogError("Decision description cannot be empty");
                Destroy(gameObject);
                return;
            }

            itemID = itemId;
            var decision = new Decision
            {
                Description = desc,
                LeftChoiceText = "Yes",
                RightChoiceText = "No",
                LeftChoice = new Choice
                {
                    StockChange = stock,
                    SatisfactionChange = satisfaction,
                    ProfitChange = profit,
                    MoraleChange = morale
                },
                RightChoice = new Choice
                {
                    StockChange = 0,
                    SatisfactionChange = 0,
                    ProfitChange = 0,
                    MoraleChange = 0
                }
            };

            ShowDecision(decision);
            Debug.Log($"Decision card setup: {desc}, itemID: {itemId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to setup DecisionCard: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public void ShowDecision(Decision decision)
    {
        try
        {
            currentDecision = decision;
            descriptionText.text = decision.Description;
            leftButtonText.text = decision.LeftChoiceText;
            rightButtonText.text = decision.RightChoiceText;

            timeRemaining = displayDuration;
            isActive = true;
            gameObject.SetActive(true);
            Debug.Log($"Showing decision: {decision.Description}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to show decision: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void OnLeftChoice()
    {
        if (!isActive) return;
        try
        {
            ApplyDecision(currentDecision.LeftChoice, "Yes");
            Debug.Log($"Left choice selected: {currentDecision.Description}");
            HideCard();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to process left choice: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void OnRightChoice()
    {
        if (!isActive) return;
        try
        {
            ApplyDecision(currentDecision.RightChoice, "No");
            Debug.Log($"Right choice selected: {currentDecision.Description}");
            HideCard();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to process right choice: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void ApplyDecision(Choice choice, string choiceMade)
    {
        try
        {
            // Update game stats
            GameManager.Instance.UpdateStats(
                choice.StockChange,
                choice.SatisfactionChange,
                choice.ProfitChange,
                choice.MoraleChange
            );

            // Log decision to database
            DatabaseManager.Instance.RecordDecision(
                playerID,
                currentDecision.Description,
                choiceMade,
                itemID,
                choice.StockChange,
                choice.SatisfactionChange,
                choice.ProfitChange,
                choice.MoraleChange
            );

            // Update stock and supplier order if applicable
            if (itemID.HasValue && choice.StockChange != 0)
            {
                DatabaseManager.Instance.UpdateItemStock(itemID.Value, -choice.StockChange); // Negative to add stock
                var item = DatabaseManager.Instance.GetItemByID(itemID.Value);
                float orderCost = item != null ? choice.StockChange * item.UnitPrice : choice.StockChange * 2.0f;
                DatabaseManager.Instance.CreateSupplierOrder(itemID.Value, choice.StockChange, orderCost, "Pending");
            }

            Debug.Log($"Applied decision: {currentDecision.Description}, choice: {choiceMade}, itemID: {itemID}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to apply decision: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void HideCard()
    {
        try
        {
            isActive = false;
            gameObject.SetActive(false);
            GameManager.Instance.NextDay();
            Debug.Log("Decision card hidden.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to hide decision card: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }
}

[System.Serializable]
public class Decision
{
    public string Description;
    public string LeftChoiceText;
    public string RightChoiceText;
    public Choice LeftChoice;
    public Choice RightChoice;
}

[System.Serializable]
public class Choice
{
    public int StockChange;
    public int SatisfactionChange;
    public int ProfitChange;
    public int MoraleChange;
}