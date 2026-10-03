using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI Panel Reference")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalTimeText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Post Processing Blur")]
    [SerializeField] private Volume globalVolume;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip buttonClickSfx;
    [SerializeField] private AudioClip buttonHoverSfx;

    private DepthOfField depthOfField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // Ensure panel is hidden at game start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Set up button click listeners and hover events
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnButtonClick);
            restartButton.onClick.AddListener(RestartGame);
            AddHoverListener(restartButton);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnButtonClick);
            quitButton.onClick.AddListener(QuitGame);
            AddHoverListener(quitButton);
        }

        // Locate Depth of Field component from Global Volume profile
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out depthOfField);
            if (depthOfField != null)
            {
                depthOfField.active = false; // Disable blur at start
            }
        }
    }

    public void TriggerGameOver()
{
    // Stop or quiet the background music on death
    if (BackgroundMusic.Instance != null)
    {
        BackgroundMusic.Instance.StopMusic(); 
        // Or lower volume: BackgroundMusic.Instance.SetVolume(0.15f);
    }

    // Enable post-processing blur
    if (depthOfField != null)
    {
        depthOfField.active = true;
    }

    // Play Game Over sound effect
    if (gameOverSfx != null && audioSource != null)
    {
        audioSource.PlayOneShot(gameOverSfx);
    }

        // Show the panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void OnButtonClick()
    {
        if (buttonClickSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSfx);
        }
    }

    private void OnButtonHover()
    {
        if (buttonHoverSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonHoverSfx);
        }
    }

    private void AddHoverListener(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((data) => { OnButtonHover(); });

        trigger.triggers.Add(entry);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}