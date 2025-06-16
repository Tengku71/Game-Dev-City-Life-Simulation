using System.Collections;
using UnityEngine;

public class TimeManagement : MonoBehaviour
{
    [Header("Toggle Objects (Optional)")]
    public GameObject[] objectsToToggle;

    [Header("Timing Settings")]
    public float timer = 0f;
    public float firstToggleTime = 300f;
    public float secondToggleTime = 600f;

    private bool firstToggleDone = false;
    private bool timerRunning = true;

    [Header("UI")]
    public GameObject restartButton; // Assign in Inspector

    public delegate void TimedEvent();
    public static event TimedEvent OnFiveMinutesPassed;
    public static event TimedEvent OnTenMinutesPassed;

    void Start()
    {
        LoadTimerState();

        // Ensure restart button is hidden at start
        if (restartButton != null)
            restartButton.SetActive(false);
    }

    void Update()
    {
        if (!timerRunning) return;

        timer += Time.deltaTime;

        if (!firstToggleDone && timer >= firstToggleTime)
        {
            HandleFiveMinuteEvent();
        }

        if (timer >= secondToggleTime)
        {
            HandleTenMinuteEvent();
        }
    }

    void HandleFiveMinuteEvent()
    {
        firstToggleDone = true;
        Debug.Log("⏱️ Event at 5 minutes triggered.");
        OnFiveMinutesPassed?.Invoke();
        StartCoroutine(ToggleObjectsWithDelay());
        SaveTimerState();
    }

    void HandleTenMinuteEvent()
    {
        timerRunning = false;
        Debug.Log("⏱️ Event at 10 minutes triggered.");
        OnTenMinutesPassed?.Invoke();
        StartCoroutine(ToggleObjectsWithDelay());

        // Show the restart button
        if (restartButton != null)
            restartButton.SetActive(true);

        SaveTimerState();
    }

    IEnumerator ToggleObjectsWithDelay()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void RestartTimer()
    {
        timer = 0f;
        firstToggleDone = false;
        timerRunning = true;

        if (restartButton != null)
            restartButton.SetActive(false);

        Debug.Log("🔄 Timer restarted.");
        SaveTimerState();
    }

    // ======= PlayerPrefs SAVE/LOAD =======

    void SaveTimerState()
    {
        PlayerPrefs.SetFloat("Timer", timer);
        PlayerPrefs.SetInt("FirstToggleDone", firstToggleDone ? 1 : 0);
        PlayerPrefs.SetInt("TimerRunning", timerRunning ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"💾 Timer saved. Time: {timer:F1}, FirstToggle: {firstToggleDone}, Running: {timerRunning}");
    }

    void LoadTimerState()
    {
        if (PlayerPrefs.HasKey("Timer"))
        {
            timer = PlayerPrefs.GetFloat("Timer");
            firstToggleDone = PlayerPrefs.GetInt("FirstToggleDone", 0) == 1;
            timerRunning = PlayerPrefs.GetInt("TimerRunning", 1) == 1;

            // Safety net: auto resume timer if it was saved too early
            if (!timerRunning && timer < secondToggleTime)
            {
                Debug.LogWarning("⚠️ Timer stopped too early. Auto-resume enabled.");
                timerRunning = true;
            }

            Debug.Log($"⏳ Loaded Timer = {timer:F1}, FirstToggle = {firstToggleDone}, Running = {timerRunning}");
        }
        else
        {
            ResetTimer();
            Debug.Log("🆕 No saved timer found. Starting fresh.");
        }
    }

    public void ResetTimer()
    {
        timer = 0f;
        firstToggleDone = false;
        timerRunning = true;

        if (restartButton != null)
            restartButton.SetActive(false);

        SaveTimerState();
    }

    [ContextMenu("🧹 Reset Timer Save (Debug)")]
    public void ClearTimerSave()
    {
        PlayerPrefs.DeleteKey("Timer");
        PlayerPrefs.DeleteKey("FirstToggleDone");
        PlayerPrefs.DeleteKey("TimerRunning");
        PlayerPrefs.Save();
        Debug.Log("🧹 Timer save data cleared.");
    }
}
