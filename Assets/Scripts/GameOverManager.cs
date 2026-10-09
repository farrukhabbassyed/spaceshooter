/*using UnityEngine;
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
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}  */


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
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Text finalTimeText;

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
    private float survivalTimer = 0f;
    private bool isGameOver = false;

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

    private void Update()
    {
        // Track time while the player is alive
        if (!isGameOver)
        {
            survivalTimer += Time.deltaTime;
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // --- STEP 1 DIAGNOSTIC LOGS ---
        Debug.Log("[GAME OVER DIAGNOSTIC] TriggerGameOver called!");
        Debug.Log($"[GAME OVER DIAGNOSTIC] finalScoreText Assigned? -> {finalScoreText != null}");
        Debug.Log($"[GAME OVER DIAGNOSTIC] finalTimeText Assigned? -> {finalTimeText != null}");

        // 1. Fetch & Set Score
        if (ScoreManager.Instance != null)
        {
            int score = ScoreManager.Instance.GetCurrentScore();
            Debug.Log($"[GAME OVER DIAGNOSTIC] Score retrieved from ScoreManager: {score}");

            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score: {score}";
            }
        }
        else
        {
            Debug.LogWarning("[GAME OVER DIAGNOSTIC] ScoreManager.Instance is NULL!");
        }

        // 2. Format & Set Survival Time (Minutes:Seconds)
        int minutes = Mathf.FloorToInt(survivalTimer / 60F);
        int seconds = Mathf.FloorToInt(survivalTimer % 60F);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        Debug.Log($"[GAME OVER DIAGNOSTIC] Survival time calculated: {formattedTime}");

        if (finalTimeText != null)
        {
            finalTimeText.text = $"Time: {formattedTime}";
        }

        // 3. Stop or quiet background music
        if (BackgroundMusic.Instance != null)
        {
            BackgroundMusic.Instance.StopMusic();
        }

        // 4. Enable post-processing blur
        if (depthOfField != null)
        {
            depthOfField.active = true;
        }

        // 5. Play Game Over SFX
        if (gameOverSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSfx);
        }

        // 6. Display the Game Over Panel
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
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}