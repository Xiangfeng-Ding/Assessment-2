using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeScoreManager : MonoBehaviour
{
    public static TimeScoreManager Instance;

    [Header("计时状态")]
    public float currentTime;
    public bool isTiming;

    [Header("UI引用")]
    public TextMeshProUGUI timeText; // UIGame里的计时文本

    public TextMeshProUGUI settleGemText; // 结算面板宝石数
    public TextMeshProUGUI settleTimeText; // 结算面板用时
    public TextMeshProUGUI settleScoreText; // 结算面板分数


    public SceneTrigger currentSceneTrigger;
    private UIManager UI;
    private int finalGemCount;
    private float finalTime;
    private int finalScore;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
     UI=UIManager.instance;   
    }
    private void Update()
    {
        if (isTiming)
        {
            currentTime += Time.deltaTime;
            UpdateTimeUI();
        }
    }

  
    /// <summary>
    /// 开始计时+宝石清零（触发区域1调用）
    /// </summary>
    public void StartTimingAndReset()
    {
        currentTime = 0;
        isTiming = true;
        GemManager.Instance.ResetGemCount(); // 联动宝石清零


    }

    /// <summary>
    /// 结束计时+跳转结算（触发区域2调用）
    /// </summary>
    public void EndTimingAndSettle()
    {
        isTiming = false;
        // 保存最终数据
        finalGemCount = GemManager.Instance.currentGemCount;
        finalTime = currentTime;
        // 计算评分：收集率*100，最低0，最高100
        float collectRate = (float)finalGemCount / GemManager.Instance.totalGemInLevel;
        finalScore = Mathf.Clamp(Mathf.RoundToInt(collectRate * 100), 0, 100);

        // 跳转面板+滚动数字
        UI.SwitchTo(UI.settlePanel);
        StartSettleScrollAnimation();
    }

    // 结算滚动数字动画
    private void StartSettleScrollAnimation()
    {
        // 先清空文本
        settleGemText.text = "0";
        settleTimeText.text = "00:00";
        settleScoreText.text = "0";

        // 启动滚动协程
        StartCoroutine(ScrollNumber(settleGemText, 0, finalGemCount, 0.8f));
        StartCoroutine(ScrollTime(settleTimeText, 0, finalTime, 1f));
        StartCoroutine(ScrollNumber(settleScoreText, 0, finalScore, 1.2f));
    }

    // 数字滚动协程
    private System.Collections.IEnumerator ScrollNumber(TextMeshProUGUI text, int start, int end, float duration)
    {
        yield return new WaitForSeconds(0.2f); // 延迟启动，错开动画
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            int current = Mathf.RoundToInt(Mathf.Lerp(start, end, timer / duration));
            text.text = current.ToString();
            yield return null;
        }
        text.text = end.ToString();
    }

    // 时间滚动协程
    private System.Collections.IEnumerator ScrollTime(TextMeshProUGUI text, float start, float end, float duration)
    {
        yield return new WaitForSeconds(0.5f);
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float current = Mathf.Lerp(start, end, timer / duration);
            // 格式化为分:秒
            int minute = Mathf.FloorToInt(current / 60);
            int second = Mathf.FloorToInt(current % 60);
            text.text = $"{minute:00}:{second:00}";
            yield return null;
        }
        // 最终时间
        int finalMinute = Mathf.FloorToInt(end / 60);
        int finalSecond = Mathf.FloorToInt(end % 60);
        text.text = $"{finalMinute:00}:{finalSecond:00}";
    }

    //内部UI更新
    private void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minute = Mathf.FloorToInt(currentTime / 60);
            int second = Mathf.FloorToInt(currentTime % 60);
            timeText.text = $"{minute:00}:{second:00}";
        }
    }

    public void MoveToNextLevel()
    {
        currentSceneTrigger.MovePlayerToTarget();
        UIManager.instance.SwitchTo(UIManager.instance.gamePanel);
    }

    public void ReloadCurrentScene()
    {
        // 获取当前激活的场景，并重新加载
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}