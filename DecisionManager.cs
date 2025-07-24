using UnityEngine;
using System.Collections.Generic;

public class DecisionManager : MonoBehaviour
{
    [SerializeField] private float timeBetweenDecisions = 30f; // Time between crisis events
    private float nextDecisionTime;
    private UIManager uiManager;
    private GameManager gameManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        nextDecisionTime = Time.time + timeBetweenDecisions;
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in scene!");
        }
    }

    private void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            ShowRandomDecision();
            nextDecisionTime = Time.time + timeBetweenDecisions;
        }
    }

    private void ShowRandomDecision()
    {
        Decision decision = GetRandomDecision();
        if (uiManager != null)
        {
            uiManager.ShowDecisionCard(decision);
        }
    }

    private Decision GetRandomDecision()
    {
        // List of possible crisis scenarios
        List<Decision> decisions = new List<Decision>
        {
            new Decision
            {
                Description = "Staff demands raises!",
                LeftChoiceText = "Approve (+Morale, -Profit)",
                RightChoiceText = "Deny (-Morale, +Profit)",
                LeftChoice = new Choice { StockChange = 0, SatisfactionChange = 0, ProfitChange = -15, MoraleChange = 20 },
                RightChoice = new Choice { StockChange = 0, SatisfactionChange = 0, ProfitChange = 10, MoraleChange = -15 }
            },
            new Decision
            {
                Description = "Customer complains about expired products!",
                LeftChoiceText = "Full Refund (+Satisfaction, -Profit)",
                RightChoiceText = "Ignore (-Satisfaction)",
                LeftChoice = new Choice { StockChange = 0, SatisfactionChange = 15, ProfitChange = -10, MoraleChange = 0 },
                RightChoice = new Choice { StockChange = 0, SatisfactionChange = -20, ProfitChange = 0, MoraleChange = 0 }
            },
            new Decision
            {
                Description = "Supplier offers bulk discount!",
                LeftChoiceText = "Buy More (+Stock, -Profit)",
                RightChoiceText = "Decline",
                LeftChoice = new Choice { StockChange = 30, SatisfactionChange = 0, ProfitChange = -20, MoraleChange = 0 },
                RightChoice = new Choice { StockChange = 0, SatisfactionChange = 0, ProfitChange = 0, MoraleChange = 0 }
            },
            new Decision
            {
                Description = "Health Inspector Visit!",
                LeftChoiceText = "Thorough Cleaning (-Profit)",
                RightChoiceText = "Quick Fix (-Satisfaction)",
                LeftChoice = new Choice { StockChange = 0, SatisfactionChange = 10, ProfitChange = -15, MoraleChange = 5 },
                RightChoice = new Choice { StockChange = 0, SatisfactionChange = -15, ProfitChange = -5, MoraleChange = -5 }
            },
            new Decision
            {
                Description = "Weekend Rush Expected!",
                LeftChoiceText = "Hire Temp Staff (-Profit, +Stock)",
                RightChoiceText = "Handle with Current Staff (-Morale)",
                LeftChoice = new Choice { StockChange = 15, SatisfactionChange = 10, ProfitChange = -15, MoraleChange = 5 },
                RightChoice = new Choice { StockChange = -10, SatisfactionChange = -5, ProfitChange = 0, MoraleChange = -15 }
            }
        };

        // Return a random decision
        return decisions[Random.Range(0, decisions.Count)];
    }
} 