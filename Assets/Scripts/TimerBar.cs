using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    
    public float timerSpeed;
    public Slider timerBar;
    public bool timerRunning;
    public GameOverManager gameOverManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerBar.value = 100;
        timerRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerBar.value > 0)
        {
            timerBar.value -= Time.deltaTime * timerSpeed;
        }
        else if(timerRunning)
        {
            timerRunning = false;
            Time.timeScale = 0f;
            gameOverManager.TriggerGameOver();
        }
    }
}
