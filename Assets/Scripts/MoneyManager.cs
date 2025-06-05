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

        StartMoneyRoutineIfNeeded();
        UpdateUI();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load money from PlayerPrefs or use initial value
        money = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetFloat("Money") : initialMoney;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //private void Start()
    //{
       
    //}

    private void Update()
    {
        // Reset money with 'R' key (for testing)
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
        //// Auto-reset if entering a "NewGame" scene
        //if (scene.name == "NewGame")
        //{
        //    ResetMoney();
        //}

        // Re-link UI
        moneyText = GameObject.FindWithTag("MoneyText")?.GetComponent<TMP_Text>();
        reductionSlider = GameObject.FindWithTag("ReductionSlider")?.GetComponent<Slider>();
        reductionValueText = GameObject.FindWithTag("ReductionValueText")?.GetComponent<TMP_Text>();
        timeSlider = GameObject.FindWithTag("TimeSlider")?.GetComponent<Slider>();
        timeValueText = GameObject.FindWithTag("TimeValueText")?.GetComponent<TMP_Text>();

        UpdateUI();
    }
}
