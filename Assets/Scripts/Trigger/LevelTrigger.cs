using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    [Header("触发类型")]
    public bool isStartTrigger; // 勾选=开始计时触发区，不勾选=结束计时触发区
    public bool ifFinalLevel;
    public AudioSource succefulAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isStartTrigger)
            {
                TimeScoreManager.Instance.StartTimingAndReset(); // 开始计时
            }
            else
            {
                TimeScoreManager.Instance.EndTimingAndSettle(); // 结束计时+结算
                succefulAudio.Play();
                if (ifFinalLevel)
                    UIManager.instance.SetRestartButton();
            }
            Destroy(gameObject);
        }
    }
}