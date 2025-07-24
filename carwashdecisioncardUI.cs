using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class carwashdecisioncardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private TextMeshProUGUI rightButtonText;
    [SerializeField] private float displayDuration = 10f; // How long the card stays before auto-choosing

    private CarWashDecision currentDecision;
    private float timeRemaining;
    private bool isActive = false;

    private void Start()
    {
        // Add button listeners
        leftButton.onClick.AddListener(OnLeftChoice);
        rightButton.onClick.AddListener(OnRightChoice);
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

    public void ShowDecision(CarWashDecision decision)
    {
        currentDecision = decision;
        descriptionText.text = decision.Description;
        leftButtonText.text = decision.LeftChoiceText;
        rightButtonText.text = decision.RightChoiceText;

        timeRemaining = displayDuration;
        isActive = true;
        gameObject.SetActive(true);
    }

    private void OnLeftChoice()
    {
        if (!isActive) return;
        ApplyDecision(currentDecision.LeftChoice);
        HideCard();
    }

    private void OnRightChoice()
    {
        if (!isActive) return;
        ApplyDecision(currentDecision.RightChoice);
        HideCard();
    }

    private void ApplyDecision(CarWashChoice choice)
    {
        CarWashGameManager.Instance.UpdateStats(
            choice.StockChange,
            choice.SatisfactionChange,
            choice.ProfitChange,
            choice.MoraleChange
        );
    }

    private void HideCard()
    {
        isActive = false;
        gameObject.SetActive(false);
        CarWashGameManager.Instance.NextDay();
    }
}