using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioClip bgmClip;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private bool playOnAwake = true;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern to ensure only one music instance plays
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource();

        if (playOnAwake && bgmClip != null)
        {
            PlayMusic();
        }
    }

    private void OnEnable()
    {
        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Restart music whenever a new scene/reload occurs
        if (bgmClip != null)
        {
            PlayMusic();
        }
    }

    private void ConfigureAudioSource()
    {
        audioSource.clip = bgmClip;
        audioSource.volume = musicVolume;
        audioSource.loop = true;          // Ensure track loops endlessly
        audioSource.spatialBlend = 0f;    // Set to 2D sound
        audioSource.playOnAwake = false;
    }

    public void PlayMusic()
    {
        if (audioSource == null) return;

        // Reset track to beginning if it was stopped on Game Over
        if (!audioSource.isPlaying)
        {
            audioSource.time = 0f;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
}