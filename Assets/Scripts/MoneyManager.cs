using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [Header("Money Settings")]
    public float initialMoney = 1000f;
    public float money;

    [Header("Dynamic Reduction Settings")]
    public float reductionAmount = 100f;
    public float timeInterval = 600f;

    [Header("UI Elements")]
    public TMP_Text moneyText;
    public Slider reductionSlider;
    public TMP_Text reductionValueText;
    public Slider timeSlider;
    public TMP_Text timeValueText;

    private Coroutine moneyRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load money FIRST
        money = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetFloat("Money") : initialMoney;

        // THEN start routines and subscribe
        StartMoneyRoutineIfNeeded();
        UpdateUI();

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Subscribe to timed events
        TimeManagement.OnFiveMinutesPassed += TriggerMidEvent;
        TimeManagement.OnTenMinutesPassed += TriggerFinalEvent;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetMoney();
        }
    }

    private void StartMoneyRoutineIfNeeded()
    {
        if (moneyRoutine == null)
            moneyRoutine = StartCoroutine(ReduceMoneyRoutine());
    }

    private IEnumerator ReduceMoneyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeInterval);
            ReduceMoney();
        }
    }

    private void ReduceMoney()
    {
        money -= reductionAmount;
        if (money < 0) money = 0;

        SaveMoney();
        Debug.Log("Money reduced. Current money: " + money);
        UpdateUI();
    }

    public void OnReductionSliderChanged()
    {
        reductionAmount = reductionSlider.value;
        if (reductionValueText) reductionValueText.text = $"Reduction: {reductionAmount}";
    }

    public void OnTimeSliderChanged()
    {
        timeInterval = timeSlider.value;
        if (timeValueText) timeValueText.text = $"Interval: {timeInterval / 60f:F1} min";

        if (moneyRoutine != null)
        {
            StopCoroutine(moneyRoutine);
            moneyRoutine = StartCoroutine(ReduceMoneyRoutine());
        }
    }

    private void UpdateUI()
    {
        if (moneyText) moneyText.text = $"{money}";
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetFloat("Money", money);
        PlayerPrefs.Save();
    }

    public void ResetMoney()
    {
        money = initialMoney;
        SaveMoney();
        UpdateUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        moneyText = GameObject.FindWithTag("MoneyText")?.GetComponent<TMP_Text>();
        reductionSlider = GameObject.FindWithTag("ReductionSlider")?.GetComponent<Slider>();
        reductionValueText = GameObject.FindWithTag("ReductionValueText")?.GetComponent<TMP_Text>();
        timeSlider = GameObject.FindWithTag("TimeSlider")?.GetComponent<Slider>();
        timeValueText = GameObject.FindWithTag("TimeValueText")?.GetComponent<TMP_Text>();

        UpdateUI();
    }

    private void TriggerMidEvent()
    {
        float rand = Random.value;
        if (rand < 0.5f)
        {
            money -= 200;
            Debug.Log("💥 Event: Resesi Ekonomi! Kehilangan 200 uang.");
        }
        else
        {
            money += 300;
            Debug.Log("🎉 Event: Peluang Investasi! Mendapatkan 300 uang.");
        }

        SaveMoney();
        UpdateUI();
    }

    private void TriggerFinalEvent()
    {
        Debug.Log("⏰ 10 menit berlalu. Akhir fase waktu. Tidak ada event tambahan.");
    }

    private void OnDestroy()
    {
        TimeManagement.OnFiveMinutesPassed -= TriggerMidEvent;
        TimeManagement.OnTenMinutesPassed -= TriggerFinalEvent;
    }
}
