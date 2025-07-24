using UnityEngine;
using System.Collections.Generic;

public class CarWashDecisionManager : MonoBehaviour
{
    [SerializeField] private float timeBetweenDecisions = 30f; // Time between decision events
    private float nextDecisionTime;
    private CarWashUIManager uiManager;
    private CarWashGameManager gameManager;

    private void Start()
    {
        uiManager = Object.FindFirstObjectByType<CarWashUIManager>();
        nextDecisionTime = Time.time + timeBetweenDecisions;
        gameManager = Object.FindFirstObjectByType<CarWashGameManager>();
        if (gameManager == null)
        {
            Debug.LogError("CarWashGameManager not found in scene!");
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
        CarWashDecision decision = GetRandomDecision();
        if (uiManager != null)
        {
            uiManager.ShowDecisionCard(decision);
        }
    }

    private CarWashDecision GetRandomDecision()
    {
        // List of possible decisions
        List<CarWashDecision> decisions = new List<CarWashDecision>
        {
            new CarWashDecision
            {
                Description = "Customer demands a free wash!",
                LeftChoiceText = "Offer free wash (+Satisfaction, -Profit)",
                RightChoiceText = "Deny request (-Satisfaction)",
                LeftChoice = new CarWashChoice { StockChange = 0, SatisfactionChange = 20, ProfitChange = -10, MoraleChange = 0 },
                RightChoice = new CarWashChoice { StockChange = 0, SatisfactionChange = -15, ProfitChange = 0, MoraleChange = 0 }
            },
            new CarWashDecision
            {
                Description = "Staff requests better equipment!",
                LeftChoiceText = "Approve purchase (+Morale, -Profit)",
                RightChoiceText = "Decline (-Morale)",
                LeftChoice = new CarWashChoice { StockChange = 0, SatisfactionChange = 0, ProfitChange = -20, MoraleChange = 15 },
                RightChoice = new CarWashChoice { StockChange = 0, SatisfactionChange = 0, ProfitChange = 0, MoraleChange = -10 }
            }
        };

        // Return a random decision
        return decisions[Random.Range(0, decisions.Count)];
    }
}