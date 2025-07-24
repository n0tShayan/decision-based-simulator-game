using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;
using System.Collections;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button settingsButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button volumeButton;
    [SerializeField] private Button settingsBackButton;
    private bool isVolumeOn = true;

    [Header("Credits Panel")]
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button creditsBackButton;

    [Header("Play Panel")]
    [SerializeField] private GameObject playPanel;
    [SerializeField] private Button superstoreButton;
    [SerializeField] private Button playBackButton;
    [SerializeField] private Animator transitionAnimator; // Assign in Inspector
    [SerializeField] private float transitionTime = 1f;   // Time before scene loads


    private void Start()
    {
        // Initialize all panels
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        playPanel.SetActive(false);

        // Set up button listeners
        playButton.onClick.AddListener(OnPlayButtonClicked);
        creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);

        // Settings panel buttons
        volumeButton.onClick.AddListener(OnVolumeButtonClicked);
        settingsBackButton.onClick.AddListener(OnSettingsBackButtonClicked);

        // Credits panel button
        creditsBackButton.onClick.AddListener(OnCreditsBackButtonClicked);

        // Play panel buttons
        superstoreButton.onClick.AddListener(OnSuperstoreButtonClicked);
        playBackButton.onClick.AddListener(OnPlayBackButtonClicked);

        // Initialize volume button text
        //UpdateVolumeButtonText();
    }

    private void OnPlayButtonClicked()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
    }

    private void OnCreditsButtonClicked()
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(true);
        Debug.Log("done boss done");
    }

    private void OnSettingsButtonClicked()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void OnVolumeButtonClicked()
    {
        isVolumeOn = !isVolumeOn;
       // UpdateVolumeButtonText();
        // Here you can add actual volume control logic
        AudioListener.volume = isVolumeOn ? 1f : 0f;
    }

   // private void UpdateVolumeButtonText()
    //{
       // volumeButton.GetComponentInChildren<Text>().text = $"Volume: {(isVolumeOn ? "On" : "Off")}";
    ///}

    public void OnSettingsBackButtonClicked()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OnCreditsBackButtonClicked()
    {
        creditsPanel.SetActive(false);
        mainPanel.SetActive(true);
        Debug.Log("done boss done");
    }

    private void OnSuperstoreButtonClicked()
    {
        StartCoroutine(LoadStoreScene());
    }

    private IEnumerator LoadStoreScene()
    {
        // Play transition animation
        transitionAnimator.SetTrigger("Start");

        // Wait for transition animation to finish
        yield return new WaitForSeconds(transitionTime);

        // Load the superstore game scene
        SceneManager.LoadScene("Store"); // Replace with your scene name
    }


    private void OnPlayBackButtonClicked()
    {
        playPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
} 