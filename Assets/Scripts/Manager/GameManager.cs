using UnityEngine;
using TMPro;

public class GemManager : MonoBehaviour
{
    public static GemManager Instance; // 单例，全局调用

    [Header("基础设置")]
    public int currentGemCount; // 当前宝石数量
    public int totalGemInLevel; // 本关卡总宝石数（用于评分）
    public int needGemToOpenDoor; // 开门需要的宝石数量

    [Header("UI引用")]
    public TextMeshProUGUI gemCountText; // UIGame里的宝石数量文本

    [Header("事件")]
    public Animator[] animator;

    public AudioSource pickUpAudio;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetGemCount(); // 开局重置宝石
    }

    //  核心方法
    /// <summary>
    /// 增加1个宝石，自动检测是否够开门
    /// </summary>
    public void AddGem()
    {

        currentGemCount++;
        pickUpAudio.Play();
        UpdateGemUI();

        // 够数量自动调用开门方法
        if (currentGemCount >= needGemToOpenDoor)
        {
            OpenDoor();
        }
    }

    /// <summary>
    /// 重置宝石数量为0
    /// </summary>
    public void ResetGemCount()
    {
        currentGemCount = 0;
        UpdateGemUI();
    }

    //  内部方法
    private void UpdateGemUI()
    {
        if (gemCountText != null)
            gemCountText.text = "Current Gem Count:"+currentGemCount.ToString();
    }

    private void OpenDoor()
    {
      
        foreach (var item in animator)
        {
            if (item != null)
            item.SetBool("Open", true);
        }
    }
}